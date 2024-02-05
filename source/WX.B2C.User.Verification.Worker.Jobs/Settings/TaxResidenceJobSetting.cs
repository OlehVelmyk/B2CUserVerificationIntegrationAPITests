using System;

namespace WX.B2C.User.Verification.Worker.Jobs.Settings
{
    internal class TaxResidenceJobSetting : UserBatchJobSettings
    {
        public Guid[] Ids { get; set; }

        public Guid TaskVariantId { get; set; }
    }
}