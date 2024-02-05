namespace WX.B2C.User.Verification.Worker.Jobs.Settings
{
    internal class CsvBlobJobSettings : BatchJobSettings
    {
        public string ContainerName { get; set; } = "jobs";

        public string FileName { get; set; }
    }
}
