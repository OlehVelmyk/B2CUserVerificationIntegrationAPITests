using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Constants;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Options;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Other;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Providers;
using WX.B2C.User.Verification.Onfido.Client.Models;
using CheckResult = WX.B2C.User.Verification.Api.Admin.Client.Models.CheckResult;
using OnfidoCheckResult = WX.B2C.User.Verification.Onfido.Client.Models.CheckResult;

namespace WX.B2C.User.Verification.Component.Tests.Mountebank.Factories
{
    internal static class OnfidoEntityFactory
    {
        public static string CreateCheck(OnfidoCheckOption[] checks)
        {
            var checkResult = checks.All(c => c.Result is CheckResult.Passed)
                ? CheckResult.Passed
                : CheckResult.Failed;

            var parameters = new Dictionary<string, object>();
            if (checkResult == CheckResult.Passed)
            {
                parameters.Add(Parameters.CheckResult, OnfidoCheckResult.Clear);
                parameters.Add(Parameters.CheckSubResult, ReportSubResult.Clear);
                return SourceProvider.GetTemplate(Templates.OnfidoCheckResponse, parameters);
            }

            parameters.Add(Parameters.CheckResult, OnfidoCheckResult.Consider);
            parameters.Add(Parameters.CheckSubResult, ReportSubResult.Suspected);
            return SourceProvider.GetTemplate(Templates.OnfidoCheckResponse, parameters);
        }

        public static string CreateReportList((OnfidoCheckOption check, string report)[] options)
        {
            var reports = options.Select(o => CreateReport(o.check, o.report)).ToList();
            var reportList = new ReportList(reports);
            return Serializer.Serialize(reportList);
        }

        private static Report CreateReport(OnfidoCheckOption check, string report)
        {
            var sourceName = check.Type switch
            {
                CheckType.IdentityDocument => Templates.OnfidoIdentityDocumentReport,
                CheckType.IdentityEnhanced => Templates.OnfidoIdentityEnhancedReport,
                CheckType.FacialSimilarity => Templates.OnfidoFacialSimilarityReport,
                CheckType.FaceDuplication => Templates.OnfidoFaceDuplicationReport,
                _ => throw new ArgumentOutOfRangeException()
            };

            var parameters = check.Type switch
            {
                CheckType.IdentityDocument => CreateIdentityDocumentReportParameters(check),
                CheckType.FacialSimilarity => CreateFacialSimilarityReportParameters(check, report),
                CheckType.FaceDuplication => CreateFaceDuplicationReportParameters(check),
                CheckType.IdentityEnhanced => CreateIdentityEnhancedReportParameters(check),
                _ => throw new ArgumentOutOfRangeException()
            };

            return SourceProvider.GetTemplate<Report>(sourceName, parameters);
        }

        private static Dictionary<string, object> CreateIdentityDocumentReportParameters(OnfidoCheckOption check)
        {
            var parameters = new Dictionary<string, object>();

            var (reportResult, fontsResult, reportSubResult) = (check.Result, check.Decision) switch
            {
                (CheckResult.Passed, _)                       => GetTuple(ReportSubResult.Clear, ReportSubResult.Clear, ReportSubResult.Clear),
                (CheckResult.Failed, CheckDecisions.Resubmit) => GetTuple(ReportResult.Consider, ReportSubResult.Clear, ReportSubResult.Rejected),
                (CheckResult.Failed, CheckDecisions.Fraud)    => GetTuple(ReportResult.Consider, ReportSubResult.Suspected, ReportSubResult.Clear),
                (CheckResult.Failed, _)                       => GetTuple(ReportResult.Consider, ReportSubResult.Clear, ReportSubResult.Rejected),
                _ => throw new ArgumentOutOfRangeException()
            };

            parameters.Add(Parameters.ReportResult, reportResult);
            parameters.Add(Parameters.ReportSubResult, reportSubResult);
            parameters.Add(Parameters.FontsResult, fontsResult);
            parameters.Add(Parameters.PoiIssuingCountry, check.PoiIssuingCountry);

            return parameters;

            (string, string, string) GetTuple<T1, T2, T3>(T1 t1, T2 t2, T3 t3) =>
                (t1.ToString(), t2.ToString(), t3.ToString());
        }

        private static Dictionary<string, object> CreateFacialSimilarityReportParameters(OnfidoCheckOption check, string report)
        {
            var parameters = new Dictionary<string, object>();

            var (result, imageIntegrityResult, faceComparison) = (check.Result, check.Decision) switch
            {
                (CheckResult.Passed, _) => (ReportResult.Clear, ReportResult.Clear, ReportResult.Clear),
                (CheckResult.Failed, CheckDecisions.Resubmit) => (ReportResult.Consider, ReportResult.Consider, ReportResult.Clear),
                (CheckResult.Failed, _) => (ReportResult.Consider, ReportResult.Clear, ReportResult.Consider),
                _ => throw new ArgumentOutOfRangeException()
            };

            parameters.Add(Parameters.ReportResult, result);
            parameters.Add(Parameters.ImageIntegrityResult, imageIntegrityResult);
            parameters.Add(Parameters.FaceComparison, faceComparison);
            parameters.Add(Parameters.ReportName, report);

            return parameters;
        }

        private static Dictionary<string, object> CreateFaceDuplicationReportParameters(OnfidoCheckOption check)
        {
            var parameters = new Dictionary<string, object>();

            var (result, imageIntegrityResult, previouslySeenFaces) = (check.Result, check.Decision) switch
            {
                (CheckResult.Passed, _) => (ReportResult.Clear, ReportResult.Clear, ReportResult.Clear),
                (CheckResult.Failed, CheckDecisions.Resubmit) => (ReportResult.Consider, ReportResult.Consider, ReportResult.Clear),
                (CheckResult.Failed, CheckDecisions.DuplicateAccount) => (ReportResult.Consider, ReportResult.Clear, ReportResult.Consider),
                (CheckResult.Failed, _) => (ReportResult.Consider, ReportResult.Clear, ReportResult.Clear),
                _ => throw new ArgumentOutOfRangeException()
            };

            parameters.Add(Parameters.ReportResult, result);
            parameters.Add(Parameters.ImageIntegrityResult, imageIntegrityResult);
            parameters.Add(Parameters.PreviouslySeenFaces, previouslySeenFaces);

            return parameters;
        }

        private static Dictionary<string, object> CreateIdentityEnhancedReportParameters(OnfidoCheckOption check) =>
            new()
            {
                [Parameters.ReportResult] = check.Result is CheckResult.Passed ? ReportResult.Clear : ReportResult.Consider
            };
    }
}
