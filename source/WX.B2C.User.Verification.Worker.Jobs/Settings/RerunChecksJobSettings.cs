using System;

namespace WX.B2C.User.Verification.Worker.Jobs.Settings
{
    internal class RerunChecksJobSettings : CsvBlobJobSettings
    {
        public Guid[] Checks { get; set; }

        public CancelRunningInstruction InstructionToCancel { get; set; }

        public RerunChecksJobSettings()
        {
            ReadingBatchSize = 1000;
            ProcessBatchSize = 100;
        }
    }

    internal class CancelRunningInstruction
    {
        public CheckError CheckError { get; set; }
    }
    
    internal class CheckError
    {
        public string Code { get; set; }

        public string Message { get; set; }
    }
}