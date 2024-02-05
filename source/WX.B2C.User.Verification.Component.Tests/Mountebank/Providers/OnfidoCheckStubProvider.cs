using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using MbDotNet.Enums;
using MbDotNet.Models.Stubs;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Constants;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Extensions;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Factories;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Options;

namespace WX.B2C.User.Verification.Component.Tests.Mountebank.Providers
{
    internal static class OnfidoCheckStubProvider
    {
        public static HttpStub CreateCheckStub(
            string applicantId,
            string bias,
            string webhookUrl,
            params (OnfidoCheckOption check, string report)[] options)
        {
            var checks = options.Select(o => o.check).ToArray();
            var reports = options.Select(o => o.report).ToArray();

            const string pathTemplate = "/{apiVersion}/checks";
            var path = pathTemplate.Replace("{apiVersion}", ApiVersions.Onfido);

            object body = applicantId is null
                ? new { report_names = reports }
                : new { applicant_id = applicantId, report_names = reports };

            var predicate = PredicateFactory.CreateEqualsPredicate(Method.Post, path, body);

            var parameters = new Dictionary<string, object>()
            {
                { Parameters.CheckIdTemplate, CreateCheckIdTemplate(EvaluateCheckCode(reports), bias) },
                { Parameters.WebhookUrl, webhookUrl },
                { Parameters.WebhookBody, SourceProvider.GetTemplate(Templates.OnfidoWebhookRequest) }
            };
            var decorateFunction = SourceProvider.GetDecorateFunction(DecorateFunctions.OnfidoCreateCheckDecorator, parameters);

            var response = OnfidoEntityFactory.CreateCheck(checks);
            return new HttpStub().On(predicate).ReturnsBody(HttpStatusCode.Created, response, decorateFunction);
        }

        public static HttpStub CreateGetCheckStub(string applicantId, string bias, params (OnfidoCheckOption check, string report)[] options)
        {
            var checks = options.Select(o => o.check).ToArray();
            var reports = options.Select(o => o.report).ToArray();

            var response = OnfidoEntityFactory.CreateCheck(checks);
            var checkCode = EvaluateCheckCode(reports);
            var checkIdPattern = CreateCheckIdPattern(checkCode, bias);

            const string pathTemplate = "^/{apiVersion}/checks/{checkId}$";
            var path = pathTemplate.Replace("{apiVersion}", ApiVersions.OnfidoPattern)
                                   .Replace("{checkId}", checkIdPattern);

            var decorateFunction = SourceProvider.GetDecorateFunction(DecorateFunctions.OnfidoGetCheckDecorator);
            var predicate = PredicateFactory.CreateMatchesPredicate(Method.Get, path);
            return new HttpStub().On(predicate).ReturnsBody(HttpStatusCode.OK, response, decorateFunction);
        }

        public static HttpStub CreateGetReportsStub(string applicantId, string bias, params (OnfidoCheckOption check, string report)[] options)
        {
            var reports = options.Select(o => o.report).ToArray();

            const string pathTemplate = "^/{@apiVersion}/reports$";
            var path = pathTemplate.Replace("{@apiVersion}", ApiVersions.OnfidoPattern);

            var response = OnfidoEntityFactory.CreateReportList(options);
            var checkCode = EvaluateCheckCode(reports);
            var checkIdPattern = CreateCheckIdPattern(checkCode, bias);
            var queryParameters = new Dictionary<string, object>() { { "check_id", checkIdPattern } };

            var predicate = PredicateFactory.CreateMatchesPredicate(Method.Get, path, queryParameters: queryParameters);
            var decorateFunction = SourceProvider.GetDecorateFunction(DecorateFunctions.OnfidoDocumentReportDecorator);
            var stub = new HttpStub().On(predicate);

            return stub.ReturnsBody(HttpStatusCode.OK, response, decorateFunction);
        }

        private static string CreateCheckIdTemplate(string checkCode, string bias)
        {
            const string CheckIdTemplate = "{checkCode}{uniquePart}{parentPart}";
            var parentPart = bias[8..];
            return CheckIdTemplate.Replace("{checkCode}", checkCode).Replace("{parentPart}", parentPart);
        }

        private static string CreateCheckIdPattern(string checkCode, string bias)
        {
            const string CheckIdPatternTemplate = "{checkCode}[0-9a-fA-F]{6}{parentPart}";
            var parentPart = bias[8..];
            return CheckIdPatternTemplate.Replace("{checkCode}", checkCode).Replace("{parentPart}", parentPart);
        }

        private static string EvaluateCheckCode(params string[] reports)
        {
            if (reports.Length > 1)
                return reports.Contains(OnfidoReports.FacialSimilarityPhoto) ? "40" : "41"; // Grouped checks

            var report = reports[0];
            return report switch
            {
                OnfidoReports.Document => "00",
                OnfidoReports.IdentityEnhanced => "10",
                OnfidoReports.FacialSimilarityPhoto => "20",
                OnfidoReports.FacialSimilarityVideo => "21",
                OnfidoReports.KnownFaces => "30",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
