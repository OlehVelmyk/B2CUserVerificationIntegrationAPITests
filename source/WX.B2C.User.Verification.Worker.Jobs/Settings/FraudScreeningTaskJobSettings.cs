using System;

namespace WX.B2C.User.Verification.Worker.Jobs.Settings
{
    internal class FraudScreeningTaskJobSettings : UserBatchJobSettings
    {

        public Guid? TaskVariantId { get; set; }
    }
}
