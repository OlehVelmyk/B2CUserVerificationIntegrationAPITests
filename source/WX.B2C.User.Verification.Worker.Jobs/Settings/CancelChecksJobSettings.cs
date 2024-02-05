using System;

namespace WX.B2C.User.Verification.Worker.Jobs.Settings
{
    internal class CancelChecksJobSettings : CsvBlobJobSettings, IEntityProvidedSettings
    {
        public CancelRunningInstruction InstructionToCancel { get; set; }
        
        public Guid[] Ids { get; set; } 
    }
}