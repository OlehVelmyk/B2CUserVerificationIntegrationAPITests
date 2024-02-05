using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class CheckRunningContextDto
    {
        public Guid UserId { get; set; }

        public Guid CheckId { get; set; }

        public Guid VariantId { get; set; }

        public CheckInputDataDto InputData { get; set; }

        public ExternalProfileDto ExternalProfile { get; set; }
    }
}