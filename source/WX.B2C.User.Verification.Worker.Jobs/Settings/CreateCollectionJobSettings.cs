using System;
using Optional;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Worker.Jobs.Settings
{
    internal class CreateStepStateJobSettings : CsvBlobJobSettings, IEntityProvidedSettings
    {
        public Guid[] Users { get; set; }

        public Guid[] Ids => Users;
        
        public TaskType[] TaskTypes { get; set; }
        
        public CollectionStepVariant Variant { get; set; }
    }

    internal class CollectionStepVariant
    {
        public string XPath { get; set; }
        
        public bool IsRequired { get; set; }

        public bool IsReviewRequired { get; set; }

        public CollectionStepState State { get; set; }

        public CollectionStepReviewResult? Result { get; set; }
    }
}