using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Events.Internal.Events;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures
{
    internal class SurveyStepFixture : CollectionStepFixture<SurveyCollectionStepVariantDto>
    {
        private readonly SurveyFixture _surveyFixture;
        private readonly EventsFixture _eventsFixture;

        public SurveyStepFixture(SurveyFixture surveyFixture, EventsFixture eventsFixture)
        {
            _surveyFixture = surveyFixture ?? throw new ArgumentNullException(nameof(surveyFixture));
            _eventsFixture = eventsFixture ?? throw new ArgumentNullException(nameof(eventsFixture));
        }

        public override Task CompleteAsync(Guid userId, SurveyCollectionStepVariantDto variant, Seed seed)
        {
            Action<Guid> assert = correlationId => _eventsFixture.ShouldExistSingle<CollectionStepCompletedEvent>(correlationId);
            return SubmitAsync(userId, variant.TemplateId, assert);
        }

        public override Task MoveInReviewAsync(Guid userId, SurveyCollectionStepVariantDto variant, Seed seed)
        {
            Action<Guid> assert = correlationId => _eventsFixture.ShouldExistSingle<CollectionStepReadyForReviewEvent>(correlationId);
            return SubmitAsync(userId, variant.TemplateId, assert);
        }

        public Task MoveInReviewAsync(Guid userId, Guid templateId)
        {
            Action<Guid> assert = correlationId => _eventsFixture.ShouldExistSingle<CollectionStepReadyForReviewEvent>(correlationId);
            return SubmitAsync(userId, templateId, assert);
        }

        private async Task SubmitAsync(Guid userId, Guid templateId, Action<Guid> assert)
        {
            var correlationId = Guid.NewGuid();
            await _surveyFixture.SubmitAsync(userId, templateId, correlationId);
            assert(correlationId);
        }
    }
}
