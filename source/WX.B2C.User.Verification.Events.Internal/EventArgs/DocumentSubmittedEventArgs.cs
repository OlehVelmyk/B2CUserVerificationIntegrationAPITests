using System;
using WX.B2C.User.Verification.Events.Internal.Dtos;
using WX.B2C.User.Verification.Events.Internal.Enums;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class DocumentSubmittedEventArgs : System.EventArgs
    {
        public Guid DocumentId { get; set; }
        
        public Guid UserId { get; set; }

        public Guid[] FilesIds { get; set; }
        
        public DocumentCategory Category { get; set; }

        public string Type { get; set; }
        
        public InitiationDto Initiation { get; set; }
        
        public static DocumentSubmittedEventArgs Create(Guid documentId,
                                                        Guid userId,
                                                        Guid[] filesIds,
                                                        DocumentCategory category,
                                                        string type,
                                                        InitiationDto initiation) =>
            new DocumentSubmittedEventArgs
            {
                DocumentId = documentId,
                UserId = userId,
                FilesIds = filesIds,
                Category = category,
                Type = type,
                Initiation = initiation
            };
    }
}