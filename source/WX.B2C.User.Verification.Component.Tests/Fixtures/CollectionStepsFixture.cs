using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Helpers;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures
{
    internal class CollectionStepsFixture
    {
        private readonly AdminApiClientFactory _adminApiClientFactory;
        private readonly AdministratorFactory _adminFactory;
        private readonly DocumentStepFixture _documentStepFixture;
        private readonly VerificationDetailsStepFixture _verificationDetailsStepFixture;
        private readonly SurveyStepFixture _surveyStepFixture;
        private readonly EventsFixture _eventsFixture;

        public CollectionStepsFixture(AdminApiClientFactory adminApiClientFactory,
                                      AdministratorFactory adminFactory,
                                      DocumentStepFixture documentStepFixture,
                                      VerificationDetailsStepFixture verificationDetailsStepFixture,
                                      SurveyStepFixture surveyStepFixture,
                                      EventsFixture eventsFixture)
        {
            _adminApiClientFactory = adminApiClientFactory ?? throw new ArgumentNullException(nameof(adminApiClientFactory));
            _adminFactory = adminFactory ?? throw new ArgumentNullException(nameof(adminFactory));
            _documentStepFixture = documentStepFixture ?? throw new ArgumentNullException(nameof(documentStepFixture));
            _verificationDetailsStepFixture = verificationDetailsStepFixture ?? throw new ArgumentNullException(nameof(verificationDetailsStepFixture));
            _surveyStepFixture = surveyStepFixture ?? throw new ArgumentNullException(nameof(surveyStepFixture));
            _eventsFixture = eventsFixture ?? throw new ArgumentNullException(nameof(eventsFixture));
        }

        public async Task<Guid> RequestAsync(Guid userId, CollectionStepRequest request)
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            request.TargetTasks ??= new List<Guid> { await GetAnyTaskId(userId) };
            await client.CollectionStep.RequestAsync(request, userId);

            var @event = _eventsFixture.ShouldExistSingle<CollectionStepRequestedEvent>(client.CorrelationId);
            request.TargetTasks.Foreach(ShouldAddTaskCollectionStep);

            return @event.EventArgs.CollectionStepId;

            void ShouldAddTaskCollectionStep(Guid taskId) =>
                _eventsFixture.ShouldExistSingle<TaskCollectionStepAddedEvent>(e => e.CorrelationId == client.CorrelationId && e.EventArgs.TaskId == taskId);
        }

        public async Task<Guid> RequestAsync(Guid userId, CollectionStepVariantDto stepVariant, bool isReviewNeeded, bool isRequired, Guid[] targetTasks = null)
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            var request = stepVariant.Map();
            request.IsReviewNeeded = isReviewNeeded;
            request.IsRequired = isRequired;
            request.TargetTasks = targetTasks ?? new Guid[] { await GetAnyTaskId(userId) };

            await client.CollectionStep.RequestAsync(request, userId);

            var @event = _eventsFixture.ShouldExistSingle<CollectionStepRequestedEvent>(client.CorrelationId);
            request.TargetTasks.Foreach(ShouldAddTaskCollectionStep);

            return @event.EventArgs.CollectionStepId;

            void ShouldAddTaskCollectionStep(Guid taskId) =>
                _eventsFixture.ShouldExistSingle<TaskCollectionStepAddedEvent>(e => e.CorrelationId == client.CorrelationId && e.EventArgs.TaskId == taskId);

        }

        public async Task UpdateAsync(Guid userId, Guid stepId, bool isReviewNeeded, bool isRequired)
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            var request = new UpdateCollectionStepRequest
            {
                IsRequired = isRequired,
                IsReviewNeeded = isReviewNeeded,
                Reason = $"{nameof(CollectionStepsFixture)}.{nameof(UpdateAsync)}"
            };

            await client.CollectionStep.UpdateAsync(request, userId, stepId);
        }

        public async Task CompleteAllAsync(Guid userId, IEnumerable<Guid> stepIds, bool completeOptional, Seed seed)
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            var steps = await client.CollectionStep.GetAllAsync(userId);
            var stepsToComplete = steps.Where(step => step.Id.In(stepIds))
                                       .Where(step => step.State is not CollectionStepState.Completed)
                                       .Where(step => completeOptional || step.IsRequired)
                                       .OrderBy(dto => dto.Variant, new TaskCollectionStepComparer());
            
            await stepsToComplete.ForeachConsistently(step => CompleteAsync(userId, step.Id, seed));
        }

        public async Task CompleteAsync(Guid userId, Guid stepId, Seed seed)
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            var step = await client.CollectionStep.GetAsync(userId, stepId);
            
            if (step.State is CollectionStepState.Completed)
                return;

            if (step.State is CollectionStepState.InReview)
            {
                await ReviewAsync(userId, stepId, CollectionStepReviewResult.Approved);
                return;
            }
            
            if (step.IsReviewNeeded)
            {
                await MoveInReviewAsync(userId, step.Variant, seed);
                await ReviewAsync(userId, stepId, CollectionStepReviewResult.Approved);
                return;
            }

            await CompleteAsync(userId, step.Variant, seed);
        }

        public Task CompleteAsync(Guid userId, CollectionStepVariantDto variant, Seed seed) =>
            variant switch
            {
                DocumentCollectionStepVariantDto documentVariant => _documentStepFixture.CompleteAsync(userId, documentVariant, seed),
                VerificationDetailsCollectionStepVariantDto verificationDetailsVariant => _verificationDetailsStepFixture.CompleteAsync(userId, verificationDetailsVariant, seed),
                SurveyCollectionStepVariantDto surveyVariant => _surveyStepFixture.CompleteAsync(userId, surveyVariant, seed),
                _ => throw new ArgumentOutOfRangeException(nameof(variant), variant, "Unsupported step variant.")
            };

        public Task MoveInReviewAsync(Guid userId, CollectionStepVariantDto variant, Seed seed) =>
            variant switch
            {
                DocumentCollectionStepVariantDto documentVariant => _documentStepFixture.MoveInReviewAsync(userId, documentVariant, seed),
                VerificationDetailsCollectionStepVariantDto verificationDetailsVariant => _verificationDetailsStepFixture.MoveInReviewAsync(userId, verificationDetailsVariant, seed),
                SurveyCollectionStepVariantDto surveyVariant => _surveyStepFixture.MoveInReviewAsync(userId, surveyVariant, seed),
                _ => throw new ArgumentOutOfRangeException(nameof(variant), variant, "Unsupported step variant.")
            };

        public async Task MoveInReviewAsync(Guid userId, Guid stepId, Seed seed)
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            var step = await client.CollectionStep.GetAsync(userId, stepId);
            if (step.State is CollectionStepState.InReview)
                return;
            if (step.State is not CollectionStepState.Requested)
                throw new InvalidOperationException();

            await MoveInReviewAsync(userId, step.Variant, seed);
        }

        public async Task ReviewAsync(Guid userId, Guid stepId, CollectionStepReviewResult reviewResult)
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            var request = new ReviewCollectionStepRequest(reviewResult, nameof(CollectionStepsFixture));
            await client.CollectionStep.ReviewAsync(request, userId, stepId);

            _eventsFixture.ShouldExistSingle<CollectionStepCompletedEvent>(client.CorrelationId);
        }

        private async Task<Guid> GetAnyTaskId(Guid userId)
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            var tasks = await client.Tasks.GetAllAsync(userId);
            return tasks.First().Id;
        }
    }
}
