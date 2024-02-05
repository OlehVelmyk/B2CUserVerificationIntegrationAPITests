using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.Core.Services.Mappers;
using WX.B2C.User.Verification.Domain.Events;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Core.Services
{
    internal class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IInitiationMapper _initiationMapper;

        public DocumentService(
            IDocumentRepository documentRepository,
            IEventPublisher eventPublisher,
            IInitiationMapper initiationMapper)
        {
            _documentRepository = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
            _initiationMapper = initiationMapper ?? throw new ArgumentNullException(nameof(initiationMapper));
        }

        public async Task SubmitAsync(Guid userId, SubmitDocumentDto submitDocumentDto, InitiationDto initiationDto)
        {
            var document = CreateDocument(userId, submitDocumentDto);
            await _documentRepository.SaveAsync(document);

            var initiation = _initiationMapper.Map(initiationDto);
            var @event = DocumentSubmitted.Create(
                document.Id,
                document.UserId,
                submitDocumentDto.FileIds,
                document.Category,
                document.Type,
                initiation);
            await _eventPublisher.PublishAsync(@event);
        }

        public async Task ArchiveAsync(Guid userId, Guid documentId, InitiationDto initiationDto)
        {
            var document = await _documentRepository.GetAsync(documentId);
            if (document.Status is DocumentStatus.Archived)
                return;

            document.Status = DocumentStatus.Archived;
            await _documentRepository.SaveAsync(document);

            var initiation = _initiationMapper.Map(initiationDto);
            var @event = DocumentArchived.Create(documentId, document.UserId, document.Category, document.Type, initiation);
            await _eventPublisher.PublishAsync(@event);
        }

        private static DocumentDto CreateDocument(Guid userId, SubmitDocumentDto submitDocumentDto)
        {
            var documentId = Guid.NewGuid();
            var files = submitDocumentDto.FileIds
                        .Select(fileId => new DocumentFileDto
                        {
                            Id = Guid.NewGuid(),
                            DocumentId = documentId,
                            FileId = fileId
                        })
                        .ToArray();

            return new DocumentDto
            {
                Id = documentId,
                UserId = userId,
                Category = submitDocumentDto.Category,
                Type = submitDocumentDto.Type,
                Status = DocumentStatus.Submitted,
                Files = files
            };
        }
    }
}