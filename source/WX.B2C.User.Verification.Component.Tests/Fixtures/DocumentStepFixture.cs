using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Component.Tests.Providers;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Extensions;
using DocumentCategory = WX.B2C.User.Verification.Component.Tests.Models.Enums.DocumentCategory;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures
{
    internal class DocumentStepFixture : CollectionStepFixture<DocumentCollectionStepVariantDto>
    {
        private readonly DocumentProvider _documentProvider;
        private readonly DocumentsFixture _documentsFixture;
        private readonly EventsFixture _eventsFixture;

        public DocumentStepFixture(DocumentProvider documentProvider, DocumentsFixture documentsFixture, EventsFixture eventsFixture)
        {
            _documentProvider = documentProvider ?? throw new ArgumentNullException(nameof(documentProvider));
            _documentsFixture = documentsFixture ?? throw new ArgumentNullException(nameof(documentsFixture));
            _eventsFixture = eventsFixture ?? throw new ArgumentNullException(nameof(eventsFixture));
        }

        public override Task CompleteAsync(Guid userId, DocumentCollectionStepVariantDto variant, Seed seed)
        {
            var category = variant.DocumentCategory.To<DocumentCategory>();
            if (variant.DocumentType is null)
                return CompleteAsync(userId, category, seed);

            Action<Guid> assert = correlationId => _eventsFixture.ShouldExistSingle<CollectionStepCompletedEvent>(correlationId);
            return SubmitAsync(userId, category, variant.DocumentType, assert, seed);
        }

        public override Task MoveInReviewAsync(Guid userId, DocumentCollectionStepVariantDto variant, Seed seed)
        {
            Action<Guid> assert = correlationId => _eventsFixture.ShouldExistSingle<CollectionStepReadyForReviewEvent>(correlationId);
            return SubmitAsync(userId, variant.DocumentCategory.To<DocumentCategory>(), assert, seed);
        }

        public Task CompleteAsync(Guid userId, DocumentCategory documentCategory, Seed seed)
        {
            Action<Guid> assert = correlationId => _eventsFixture.ShouldExistSingle<CollectionStepCompletedEvent>(correlationId);
            return SubmitAsync(userId, documentCategory, assert, seed);
        }

        private async Task SubmitAsync(Guid userId, DocumentCategory category, Action<Guid> assert, Seed seed)
        {
            var correlationId = Guid.NewGuid();
            await _documentsFixture.SubmitAsync(userId, category, seed, correlationId);
            assert(correlationId);
        }

        private async Task SubmitAsync(Guid userId, DocumentCategory category, string type, Action<Guid> assert, Seed seed)
        {
            var correlationId = Guid.NewGuid();
            var documentToSubmit = _documentProvider.GetDocumentToSubmit(category, type, 1, "jpg", seed); // TODO: Hardcode!
            await _documentsFixture.SubmitByAdminAsync(userId, documentToSubmit, correlationId);
            assert(correlationId);
        }
    }
}
