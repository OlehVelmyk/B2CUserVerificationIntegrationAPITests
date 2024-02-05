using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Quartz;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Models.Verification;
using WX.B2C.User.Verification.Worker.Jobs.Providers.CollectionSteps;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.CollectionSteps
{
    internal class SurveyCollectionStepsJob : BatchJob<UserSurveyChecks, CollectionStepsJobSettings>
    {
        private static readonly Dictionary<SurveyCheckType, string> CheckTypeToXPathMappingDictionary = new()
        {
            { SurveyCheckType.Onboarding, "Survey.C5E7A138-2E36-43D0-BD76-43A606068F49" },
            { SurveyCheckType.Pep, "Survey.CA6B7FB1-413D-449B-9038-32AB5B4914B6" },
            { SurveyCheckType.Occupation, "Survey.F9A2A3AC-6E98-43C9-BAB2-794E8E6DF686" },
            { SurveyCheckType.Cdd, "Survey.DE532CA0-C21E-4F7B-AD09-647EAA0C4E00" },
            { SurveyCheckType.Edd, "Survey.EDDACA4C-C4A6-40C6-8FF3-D63A5D435783" },
            { SurveyCheckType.SourceOfFunds, "Survey.0FB7492B-7DC5-4277-A7FF-F3D07376FF66" },
        };

        private readonly ICollectionStepCreatorFactory _creatorFactory;
        private readonly IApplicationStorage _applicationStorage;
        private ICollectionStepsCreator _creator;

        public SurveyCollectionStepsJob(ICollectionStepCreatorFactory creatorFactory,
                                        IApplicationStorage applicationStorage,
                                        IUserSurveyChecksProvider jobDataProvider,
                                        ILogger logger) : base(jobDataProvider, logger)
        {
            _creatorFactory = creatorFactory ?? throw new ArgumentNullException(nameof(creatorFactory));
            _applicationStorage = applicationStorage ?? throw new ArgumentNullException(nameof(applicationStorage));
        }

        public static string Name => "survey-collection-step-backfill";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<SurveyCollectionStepsJob>()
                 .WithIdentity(Name, "Backfill")
                 .WithDescription("Backfill collection steps according to documents checks and verification stop reason");

        protected override async Task Execute(Batch<UserSurveyChecks> batch,
                                              CollectionStepsJobSettings settings,
                                              CancellationToken cancellationToken)
        {
            var logger = Logger.ForContext("JobName", Name);
            _creator ??= _creatorFactory.Create(Name,
                                                xPathes => Filter(xPathes, settings.ExcludedXPathes),
                                                settings.UseActors);
            foreach (var data in batch.Items)
            {
                var userId = data.UserId;
                if (data.Checks.IsNullOrEmpty())
                    continue;
                
                logger = logger.ForContext(nameof(data.UserId), userId);
                
                var application = await _applicationStorage.FindAsync(userId, ProductType.WirexBasic);
                if (application == null)
                {
                    logger.Warning("No application found for user. Creation collection steps skipped.");
                    continue;
                }
                
                logger = logger.ForContext(nameof(application.PolicyId), application.PolicyId);

                var expectedSteps = GetExpectedStepsStates(data);

                var tasks = application.Tasks.Select(dto => new CollectionStepsCreator.TaskInfo
                                       {
                                           Id = dto.Id,
                                           Type = dto.Type,
                                           VariantId = dto.VariantId
                                       })
                                       .ToArray();

                try
                {
                    await _creator.CreateStepsAsync(userId, application.PolicyId, expectedSteps, tasks);
                }
                catch (Exception e)
                {
                    logger.Error(e, "Exception during creating collection steps");
                    IncrementErrorCount();
                }
                
            }
        }

        private IEnumerable<string> Filter(IEnumerable<string> xPathes, string[] excludedXPathes) =>
            excludedXPathes.IsNullOrEmpty() 
                ? xPathes.Intersect(CheckTypeToXPathMappingDictionary.Values) 
                : xPathes.Intersect(CheckTypeToXPathMappingDictionary.Values).Except(excludedXPathes);

        private Dictionary<string, NewCollectionStep> GetExpectedStepsStates(UserSurveyChecks userSurveyChecks)
        {
            var expectedStepsState = new Dictionary<string, NewCollectionStep>();
            foreach (var surveyCheck in userSurveyChecks.Checks)
            {
                var xPath = GetSurveyXPath(surveyCheck);
                var state = GetSurveyState(surveyCheck);
                if (state == CollectionStepState.Completed)
                {
                    var result = surveyCheck.Result == SurveyCheckResult.Clear
                        ? CollectionStepReviewResult.Approved
                        : CollectionStepReviewResult.Rejected;
                    expectedStepsState.Add(xPath, NewCollectionStep.ReviewCompleted(result));
                    continue;
                }

                expectedStepsState.Add(xPath, NewCollectionStep.Incompleted(state));
            }

            return expectedStepsState;
        }

        private CollectionStepState GetSurveyState(SurveyCheck surveyCheck) =>
            surveyCheck.Status switch
            {
                SurveyCheckStatus.Requested => CollectionStepState.Requested,
                SurveyCheckStatus.Assigned  => CollectionStepState.Requested,
                SurveyCheckStatus.OnReview  => CollectionStepState.InReview,
                SurveyCheckStatus.Completed => CollectionStepState.Completed,
                _                           => throw new ArgumentOutOfRangeException()
            };

        private static string GetSurveyXPath(SurveyCheck checksDataCheck) =>
            CheckTypeToXPathMappingDictionary[checksDataCheck.Type];
    }
}