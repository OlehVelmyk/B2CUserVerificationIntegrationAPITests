using System;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class CheckProcessingContextDto
    {
        public Guid UserId { get; set; }

        public Guid CheckId { get; set; }

        public Guid VariantId { get; set; }

        public CheckExternalDataDto ExternalData { get; set; }
    }
}