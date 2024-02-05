using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MbDotNet;
using MbDotNet.Enums;
using MbDotNet.Models.Stubs;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Abstraction;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Constants;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Extensions;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Factories;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Options;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Providers;
using PassfortCheckResult = WX.B2C.User.Verification.PassFort.Client.Models.CheckResult;

namespace WX.B2C.User.Verification.Component.Tests.Mountebank
{
    internal interface IPassfortImposter : IHttpImposter
    {
        Task ConfigureCheckAsync(PassfortCheckOptions checkOptions);
    }

    internal class PassfortImposter : ProxyHttpImposter, IPassfortImposter
    {
        private readonly string _webhookUrl;

        public PassfortImposter(MountebankClient client, string url, string proxyUrl, string webhookUrl)
            : base(client, url, proxyUrl)
        {
            _webhookUrl = webhookUrl ?? throw new ArgumentNullException(nameof(webhookUrl));
        }

        protected override IEnumerable<HttpStub> CreateCustomDefaultStubs()
        {
            var stubs = new List<HttpStub>();
            var defaultOptions = PassfortCheckOptions.Passed();

            stubs.Add(CreateDefaultRunCheckStub(defaultOptions));
            stubs.Add(CreateDefaultGetCheckStub(defaultOptions));
            stubs.Add(CreateDefaultUpdateProfileStub());

            return stubs;
        }

        public Task ConfigureCheckAsync(PassfortCheckOptions checkOptions)
        {
            if(checkOptions is null)
                throw new ArgumentNullException(nameof(checkOptions));
            if(checkOptions.ProfileId is null)
                throw new ArgumentNullException(nameof(checkOptions.ProfileId));

            var stubs = new List<HttpStub>();
            stubs.Add(CreateRunCheckStub(checkOptions));
            stubs.Add(CreateGetCheckStub(checkOptions));

            return AppendStubsAsync(stubs);
        }

        private HttpStub CreateRunCheckStub(PassfortCheckOptions checkOptions)
        {
            const string pathTemplate = "/{apiVersion}/profiles/{profileId}/checks";
            var path = pathTemplate.Replace("{apiVersion}", ApiVersions.Passfort)
                                   .Replace("{profileId}", checkOptions.ProfileId);

            string response = CreateCheckResponse(checkOptions);
            var decorateFunction = CreateRunCheckDecorator();
            var predicate = PredicateFactory.CreateDeepEqualsPredicate(Method.Post, path);

            return new HttpStub().On(predicate).ReturnsBody(HttpStatusCode.OK, response, decorateFunction);
        }

        private HttpStub CreateGetCheckStub(PassfortCheckOptions checkOptions)
        {
            const string pathTemplate = "^/{apiVersion}/profiles/{profileId}/checks/{checkId}$";
            var path = pathTemplate.Replace("{apiVersion}", ApiVersions.PassfortPattern)
                                   .Replace("{profileId}", checkOptions.ProfileId)
                                   .Replace("{checkId}", Parameters.AnyGuidPattern);

            string response = CreateCheckResponse(checkOptions);
            var predicate = PredicateFactory.CreateMatchesPredicate(Method.Get, path);

            return new HttpStub().On(predicate).ReturnsBody(HttpStatusCode.OK, response);
        }

        private HttpStub CreateDefaultRunCheckStub(PassfortCheckOptions checkOptions)
        {
            const string pathTemplate = "^/{apiVersion}/profiles/{profileId}/checks$";
            var path = pathTemplate.Replace("{apiVersion}", ApiVersions.PassfortPattern)
                                   .Replace("{profileId}", Parameters.AnyGuidPattern);

            var response = CreateCheckResponse(checkOptions);
            var decorateFunction = CreateRunCheckDecorator();
            var predicate = PredicateFactory.CreateMatchesPredicate(Method.Post, path);

            return new HttpStub().On(predicate).ReturnsBody(HttpStatusCode.OK, response, decorateFunction);
        }

        private HttpStub CreateDefaultGetCheckStub(PassfortCheckOptions checkOptions)
        {
            const string pathTemplate = "^/{apiVersion}/profiles/{profileId}/checks/{checkId}$";
            var path = pathTemplate.Replace("{apiVersion}", ApiVersions.PassfortPattern)
                                   .Replace("{profileId}", Parameters.AnyGuidPattern)
                                   .Replace("{checkId}", Parameters.AnyGuidPattern);

            var response = CreateCheckResponse(checkOptions);
            var predicate = PredicateFactory.CreateMatchesPredicate(Method.Get, path);

            return new HttpStub().On(predicate).ReturnsBody(HttpStatusCode.OK, response);
        }

        private HttpStub CreateDefaultUpdateProfileStub()
        {
            const string pathTemplate = "^/{apiVersion}/profiles/{profileId}/collected_data$";
            var path = pathTemplate.Replace("{apiVersion}", ApiVersions.PassfortPattern)
                                   .Replace("{profileId}", Parameters.AnyGuidPattern);

            var response = SourceProvider.GetTemplate(Templates.PassfortUpdateProfileResponse);
            var predicate = PredicateFactory.CreateMatchesPredicate(Method.Post, path);
            return new HttpStub().On(predicate).ReturnsBody(HttpStatusCode.OK, response);
        }

        private string CreateRunCheckDecorator()
        {
            var parameters = new Dictionary<string, object>()
            {
                { Parameters.WebhookUrl, _webhookUrl },
                { Parameters.WebhookBody, SourceProvider.GetTemplate(Templates.PassfortWebhookRequest) }
            };
            return SourceProvider.GetDecorateFunction(DecorateFunctions.PassfortRunCheckDecorator, parameters);
        }

        private string CreateCheckResponse(PassfortCheckOptions checkOptions)
        {
            if (checkOptions.Result == CheckResult.Passed)
                return CreateCheckResponse(PassfortCheckResult.Pass);
            else if (checkOptions.IsSanctioned)
                return CreateCheckResponse(PassfortCheckResult.Sanction);
            else if (checkOptions.IsPep)
                return CreateCheckResponse(PassfortCheckResult.PEP);
            else if (checkOptions.IsAdverseMedia)
                return CreateCheckResponse(PassfortCheckResult.Media);
            else
                return CreateCheckResponse(PassfortCheckResult.Fail);
        }

        private string CreateCheckResponse(string checkResult)
        {
            var parameters = new Dictionary<string, object>() { { Parameters.CheckResult, checkResult } };
            return SourceProvider.GetTemplate(Templates.PassfortCheckResponse, parameters);
        }
    }
}
