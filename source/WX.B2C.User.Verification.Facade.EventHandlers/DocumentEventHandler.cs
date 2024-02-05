using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Extensions;
using WX.Messaging.Subscriber.HandlerResolving;

namespace WX.B2C.User.Verification.Facade.EventHandlers
{
    internal class DocumentEventHandler : BaseEventHandler, 
                                          IEventHandler<DocumentSubmittedEvent>
    {
        private readonly IDocumentStorage _documentStorage;
        private readonly IDocumentService _documentService;

        public DocumentEventHandler(
            IDocumentStorage documentStorage, 
            IDocumentService documentService, 
            EventHandlingContext context) : base(context)
        {
            _documentStorage = documentStorage ?? throw new ArgumentNullException(nameof(documentStorage));
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
        }

        public Task HandleAsync(DocumentSubmittedEvent @event) =>
            Handle(@event, async args =>
            {
                var initiation = InitiationDto.CreateSystem(InitiationReasons.ArchiveDocument);
                var categories = new[] { args.Category.To<DocumentCategory>() };
                var submittedDocuments = await _documentStorage.FindSubmittedDocumentsAsync(args.UserId, categories);
                await submittedDocuments
                      .OrderByDescending(document => document.CreatedAt)
                      .Skip(1)
                      .Foreach(ArchiveAsync);

                Task ArchiveAsync(DocumentDto document) =>
                    _documentService.ArchiveAsync(document.UserId, document.Id, initiation);
            });
    }
}
