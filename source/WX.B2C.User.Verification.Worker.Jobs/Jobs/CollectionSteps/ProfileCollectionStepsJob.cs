using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Models.CollectionSteps;
using WX.B2C.User.Verification.Worker.Jobs.Providers.CollectionSteps;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.CollectionSteps
{
    internal class ProfileCollectionStepsJob : BatchJob<ProfileDataExistence, CollectionStepsJobSettings>
    {
        private static readonly string[] ProcessableXPathes = 
        {
            XPathes.FullName,
            XPathes.Birthdate,
            XPathes.ResidenceAddress,
            XPathes.IpAddress,
            XPathes.TaxResidence,
            XPathes.IdDocumentNumber,
            XPathes.Tin,
            XPathes.VerifiedNationality
        };

        private readonly ITaskStorage _taskStorage;
        private readonly ICollectionStepCreatorFactory _creatorFactory;
        private ICollectionStepsCreator _creator;

        public ProfileCollectionStepsJob(IProfileDataExistenceProvider dataProvider,
                                         ITaskStorage taskStorage,
                                         ICollectionStepCreatorFactory collectionStepCreatorFactory,
                                         ILogger logger)
            : base(dataProvider, logger)
        {
            _taskStorage = taskStorage ?? throw new ArgumentNullException(nameof(taskStorage));
            _creatorFactory = collectionStepCreatorFactory ?? throw new ArgumentNullException(nameof(collectionStepCreatorFactory));
        }

        public static string Name => "profile-collection-step-backfill";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<ProfileCollectionStepsJob>()
                 .WithIdentity(Name, "Backfill")
                 .WithDescription("Backfill collection steps according to data existence and attach to tasks");

        protected override async Task Execute(Batch<ProfileDataExistence> batch,
                                              CollectionStepsJobSettings settings,
                                              CancellationToken cancellationToken)
        {
            _creator ??= _creatorFactory.Create(Name,
                                                xPathes => Filter(xPathes, settings.ExcludedXPathes),
                                                settings.UseActors);
            foreach (var data in batch.Items)
            {
                var userId = data.UserId;

                var expectedSteps = GetExpectedSteps(data);
                var tasks = (await _taskStorage.GetAllAsync(userId))
                            .Select(dto => new CollectionStepsCreator.TaskInfo
                            {
                                Id = dto.Id,
                                Type = dto.Type,
                                VariantId = dto.VariantId
                            })
                            .ToArray();

                try
                {
                    await _creator.CreateStepsAsync(userId, data.PolicyId, expectedSteps, tasks);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Exception during creating collection steps");
                    IncrementErrorCount();
                }
            }
        }

        private IEnumerable<string> Filter(IEnumerable<string> xPathes, string[] excludedXPathes)
        {
            if (excludedXPathes.IsNullOrEmpty())
                return xPathes.Intersect(ProcessableXPathes);
            return xPathes.Intersect(ProcessableXPathes).Except(excludedXPathes);
        }

        private Dictionary<string, NewCollectionStep> GetExpectedSteps(ProfileDataExistence dataExistence)
        {
            var expectedStepsState = new Dictionary<string, NewCollectionStep>();
            var isApproved = dataExistence.State == ApplicationState.Approved;
            
            AddCompleted(XPathes.TaxResidence, dataExistence.TaxResidence, isApproved);
            AddCompleted(XPathes.Tin, dataExistence.Tin, isApproved);
            
            var inReview = dataExistence.State == ApplicationState.InReview;
            
            // According to logic of Roman,
            // we should not require steps which user can't fill from app if user was verified.
            
            AddCompleted(XPathes.FullName, dataExistence.FullName, isApproved || inReview);
            AddCompleted(XPathes.Birthdate, dataExistence.DateOfBirth, isApproved || inReview);
            AddCompleted(XPathes.ResidenceAddress, dataExistence.Address, isApproved || inReview);
            AddCompleted(XPathes.IpAddress, dataExistence.IpAddress, isApproved || inReview);
            AddCompleted(XPathes.IdDocumentNumber, dataExistence.IdDocumentNumber, isApproved || inReview);
            AddCompleted(XPathes.VerifiedNationality, dataExistence.Nationality, isApproved  || inReview);
            
            return expectedStepsState;

            void AddCompleted(string xPath, bool isExists, bool isOptional) =>
                expectedStepsState.Add(xPath,
                                       isExists
                                           ? NewCollectionStep.Completed()
                                           : NewCollectionStep.Incompleted(CollectionStepState.Requested, isOptional));
        }
    }
}