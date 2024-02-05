using System;

namespace WX.B2C.User.Verification.Worker.Jobs.Settings
{
    internal class TriggerManagingSettings : CsvBlobJobSettings
    {
        public TriggerManagingSettings()
        {
            DelayInMillisecondsAfterBatch = 5_000;
            ProcessBatchSize = 1;
        }

        public string[] Actions { get; set; }

        public Guid TriggerPolicyId { get; set; }
    }
}