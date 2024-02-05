namespace WX.B2C.User.Verification.Worker.Jobs.Settings
{
    internal class InstructChecksJobSettings : CsvBlobJobSettings
    {
        public bool DirectUpdate { get; set; }

        public InstructChecksJobSettings()
        {
            ReadingBatchSize = 1000;
            ProcessBatchSize = 100;
        }
    }
}
