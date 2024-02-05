using System;

namespace WX.B2C.User.Verification.Worker.Jobs.Settings
{
    internal class ApplicationJobSettings : CsvBlobJobSettings, IEntityProvidedSettings
    {
        public Guid[] Applications { get; set; }

        public Guid[] Ids => Applications;
    }
}