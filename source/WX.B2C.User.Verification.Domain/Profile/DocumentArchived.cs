using System;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Events
{
    public class DocumentArchived : DomainEvent
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public DocumentCategory DocumentCategory { get; private set; }

        public string DocumentType { get; private set; }

        public Initiation Initiation { get; private set; }

        public static DocumentArchived Create(Guid documentId,
                                              Guid userId,
                                              DocumentCategory documentCategory,
                                              string documentType,
                                              Initiation initiation) =>
            new()
            {
                Id = documentId,
                UserId = userId,
                DocumentCategory = documentCategory,
                DocumentType = documentType,
                Initiation = initiation
            };
    }
}
