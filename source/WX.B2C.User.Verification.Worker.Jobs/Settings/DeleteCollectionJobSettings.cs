using System;
using Optional;
using WX.B2C.User.Verification.Domain.DataCollection;

namespace WX.B2C.User.Verification.Worker.Jobs.Settings
{
    internal class DeleteStepStateJobSettings : CsvBlobJobSettings, IEntityProvidedSettings
    {
        public Guid[] Ids { get; set; }
    }
}