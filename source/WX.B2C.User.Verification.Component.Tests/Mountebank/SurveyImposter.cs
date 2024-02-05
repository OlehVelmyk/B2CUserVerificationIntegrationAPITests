using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MbDotNet;
using MbDotNet.Enums;
using MbDotNet.Models.Stubs;
using WX.B2C.Survey.Api.Internal.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Abstraction;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Constants;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Factories;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Providers;

namespace WX.B2C.User.Verification.Component.Tests.Mountebank
{
    // TODO: Contracts require refinement
    internal interface ISurveyImposter : IHttpImposter
    {
        Task ConfigureGetAsync(Guid userId, Guid templateId, bool isSubmitted);

        Task ConfigureGetAnswersAsync(Guid userId, Guid templateId, TaggedAnswerDto[] taggedAnswers = null);
    }

    internal class SurveyImposter : DefaultHttpImposter, ISurveyImposter
    {
        private const string GetPathTemplate = @"^/api/v1/users/{userId}/surveys/{templateId}$";
        private const string GetAnswersPathTemplate = @"^/api/v1/users/{userId}/surveys/{templateId}/answers$";

        public SurveyImposter(MountebankClient client, string b2cSurveyUrl) 
            : base(client, b2cSurveyUrl)
        {  }

        protected override IEnumerable<HttpStub> CreateCustomDefaultStubs()
        {
            var stubs = new List<HttpStub>();
            stubs.Add(CreateDefaultGetStub());
            stubs.Add(CreateDefaultGetAnswersStub());

            return stubs;
        }

        public Task ConfigureGetAsync(Guid userId, Guid templateId, bool isSubmitted)
        {
            var path = GetPathTemplate.Replace("{userId}", userId.ToString())
                                      .Replace("{templateId}", templateId.ToString());

            var response = SourceProvider.GetTemplate<UserSurveyDto>(Templates.UserSurveyTemplate);

            response.IsSubmitted = isSubmitted;
            response.UserId = userId;
            response.SurveyId = templateId;
            response.SubmitDate = DateTime.UtcNow;

            var predicate = PredicateFactory.CreateMatchesPredicate(Method.Get, path);
            var stub = new HttpStub().On(predicate).ReturnsJson(HttpStatusCode.OK, response);

            return AppendStubAsync(stub);
        }

        public Task ConfigureGetAnswersAsync(Guid userId, Guid templateId, TaggedAnswerDto[] taggedAnswers = null)
        {
            var path = GetAnswersPathTemplate.Replace("{userId}", userId.ToString())
                                             .Replace("{templateId}", templateId.ToString());

            var response = taggedAnswers ?? SourceProvider.GetTemplate<TaggedAnswerDto[]>(Templates.TaggedAnswersTemplate);
            var tags = response.SelectMany(answer => answer.Tags);
            var query = new Dictionary<string, object>() { { "tags", string.Join(',', tags) } };

            var predicate = PredicateFactory.CreateMatchesPredicate(Method.Get, path, queryParameters: query);
            var stub = new HttpStub().On(predicate).ReturnsJson(HttpStatusCode.OK, response);

            return AppendStubAsync(stub);
        }

        private HttpStub CreateDefaultGetStub()
        {
            var path = GetPathTemplate.Replace("{userId}", Parameters.AnyGuidPattern)
                                      .Replace("{templateId}", Parameters.AnyGuidPattern);

            var response = SourceProvider.GetTemplate<UserSurveyDto>(Templates.UserSurveyTemplate);
            response.SubmitDate = DateTime.UtcNow;

            var predicate = PredicateFactory.CreateMatchesPredicate(Method.Get, path);
            return new HttpStub().On(predicate).ReturnsJson(HttpStatusCode.OK, response);
        }

        private HttpStub CreateDefaultGetAnswersStub()
        {
            var path = GetAnswersPathTemplate.Replace("{userId}", Parameters.AnyGuidPattern)
                                             .Replace("{templateId}", Parameters.AnyGuidPattern);

            var query = new Dictionary<string, object>() { { "tags", ".+" } };
            var response = SourceProvider.GetTemplate(Templates.TaggedAnswersTemplate);

            var predicate = PredicateFactory.CreateMatchesPredicate(Method.Get, path, queryParameters: query);
            return new HttpStub().On(predicate).ReturnsBody(HttpStatusCode.OK, response);
        }
    }
}
