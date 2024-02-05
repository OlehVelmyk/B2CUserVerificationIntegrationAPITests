using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.Exceptions;
using WX.B2C.User.Verification.Provider.Contracts.Models;
using ErrorCodes = WX.B2C.User.Verification.Provider.Contracts.Constants.ErrorCodes;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    internal class SurveyAnswersCheckRunner : SyncCheckRunner<SurveyAnswersCheckInputData>
    {
        private readonly SurveyAnswersCheckConfiguration _configuration;
        private readonly IUserSurveyProvider _surveyProvider;

        public SurveyAnswersCheckRunner(
            SurveyAnswersCheckConfiguration configuration,
            IUserSurveyProvider surveyProvider)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _surveyProvider = surveyProvider ?? throw new ArgumentNullException(nameof(surveyProvider));
        }

        protected override async Task<CheckProcessingResult> RunSync(SurveyAnswersCheckInputData data)
        {
            var riskAnswers = _configuration.RiskAnswers;
            if (riskAnswers == null)
                throw new CheckExecutionException(ErrorCodes.ConfigurationError, "Risk answers is not configured for check.");

            var questionTags = riskAnswers.Keys.ToList();
            var taggedAnswers = await _surveyProvider.GetAnswersAsync(data.UserId, data.TemplateId, questionTags);

            var matches = riskAnswers
                .ToDictionary(
                    x => x.Key,
                    x => GetMatches(x.Key, taggedAnswers, x.Value));

            var hasRiskMatches = matches.Any(x => x.Value.Any());

            var outputData = new SurveyAnswersCheckOutputData
            {
                Matches = matches
            };

            return !hasRiskMatches
                ? CheckProcessingResult.Passed(outputData)
                : CheckProcessingResult.Failed(outputData);
        }

        private static string[] GetMatches(string questionTag, IEnumerable<TaggedAnswer> taggedAnswers, IEnumerable<string> riskAnswers)
        {
            return taggedAnswers
                   .Where(answer => answer.Tags.Contains(questionTag))
                   .SelectMany(answer => answer.Values)
                   .Where(riskAnswers.Contains)
                   .ToArray();
        }
    }
}
