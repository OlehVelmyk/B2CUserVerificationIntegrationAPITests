using System;

namespace WX.B2C.User.Verification.DataAccess.Entities
{
    internal class DocumentFile
    {
        // Document can have duplicate files so DocumentId + FileId can not be PK
        public Guid Id { get; set; }

        public Guid DocumentId { get; set; }

        public Guid FileId { get; set; }

        public virtual File File { get; set; }
        
        public virtual Document Document { get; set; }
    }
}
