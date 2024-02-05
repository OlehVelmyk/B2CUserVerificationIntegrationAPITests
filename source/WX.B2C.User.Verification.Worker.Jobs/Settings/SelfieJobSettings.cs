using System;

namespace WX.B2C.User.Verification.Worker.Jobs.Settings
{
    internal class SelfieJobSettings : CsvBlobJobSettings
    {
        public int OnfidoRequestPerMinute { get; set; } = 100;
        
        public Guid[] Users { get; set; }
    }
}
