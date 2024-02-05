using System;
using WX.B2C.User.Verification.Worker.Jobs.Models;

namespace WX.B2C.User.Verification.Worker.Jobs.Settings
{
    internal class CollectionStepsJobSettings : CsvBlobJobSettings
    {
        public Guid[] Users { get; set; }

        public string[] ExcludedXPathes { get; set; }

        public bool UseActors { get; set; }
    }
}