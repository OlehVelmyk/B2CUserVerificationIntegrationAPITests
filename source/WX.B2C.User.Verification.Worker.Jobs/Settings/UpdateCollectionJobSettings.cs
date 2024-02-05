using System;
using Optional;
using WX.B2C.User.Verification.Domain.DataCollection;

namespace WX.B2C.User.Verification.Worker.Jobs.Settings
{
    internal class UpdateStepStateJobSettings : CsvBlobJobSettings, IEntityProvidedSettings
    {
        public Guid[] Ids { get; set; }
        
        public CollectionStepUpdatePatch Patch { get; set; }
    }

    internal class CollectionStepUpdatePatch
    {
        public Option<bool> IsRequired { get; set; }

        public Option<bool> IsReviewNeeded { get; set; }

        public Option<CollectionStepState> State { get; set; }

        public Option<CollectionStepReviewResult?> ReviewResult { get; set; }
    }
}