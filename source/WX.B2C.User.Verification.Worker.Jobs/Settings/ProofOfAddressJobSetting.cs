using System;

namespace WX.B2C.User.Verification.Worker.Jobs.Settings
{
    internal class ProofOfAddressJobSetting : UserBatchJobSettings
    {
        public Guid[] Ids { get; set; }

        public Guid TaskVariantId { get; set; }
    }
}