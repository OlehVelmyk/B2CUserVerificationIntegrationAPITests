using System;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Events
{
    public class DocumentSubmitted : DomainEvent
    {
        public Guid DocumentId { get; private set; }
        
        public Guid[] FilesIds { get; private set; }
        
        public Guid UserId { get; private set; }

        public DocumentCategory DocumentCategory { get; private set; }

        public string DocumentType { get; private set; }

        public Initiation Initiation { get; private set; }

        public static DocumentSubmitted Create(Guid documentId, 
                                               Guid userId,
                                               Guid[] filesIds,
                                               DocumentCategory documentCategory,
                                               string documentType,
                                               Initiation initiation) =>
            new()
             {
                 DocumentId = documentId,
                 UserId = userId,
                 FilesIds = filesIds,
                 DocumentCategory = documentCategory,
                 DocumentType = documentType, 
                 Initiation = initiation
             };
    }
}