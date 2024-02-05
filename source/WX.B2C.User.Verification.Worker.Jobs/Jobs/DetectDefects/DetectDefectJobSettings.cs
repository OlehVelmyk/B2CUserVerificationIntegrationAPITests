using System;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects
{
    internal class DetectDefectJobSettings : CsvBlobJobSettings
    {
        public string ReportContainer { get; set; } = "reports";

        public string ReportName { get; set; } = $"verification-consistency-report-{DateTime.UtcNow:yyMMddHHmmss}.csv";

        public int ReportLinkTtl { get; set; } = 30;

        public Guid[] Users { get; set; }

        public string[] IgnoredDefects { get; set; } = Array.Empty<string>();
        
        public string UserPredicate { get; set; }
    }
}