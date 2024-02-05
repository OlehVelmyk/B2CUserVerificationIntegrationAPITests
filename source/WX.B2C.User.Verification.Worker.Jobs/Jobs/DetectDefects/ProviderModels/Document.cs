using System;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.ProviderModels
{
    internal class Document
    {
        public Guid UserId { get; set; }

        public DocumentCategory Category { get; set; }
    }
}