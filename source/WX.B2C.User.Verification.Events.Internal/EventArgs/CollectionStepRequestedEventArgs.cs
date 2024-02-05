using System;
using WX.B2C.User.Verification.Events.Internal.Dtos;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class CollectionStepRequestedEventArgs : System.EventArgs
    {
        public Guid CollectionStepId { get; set; }

        public Guid UserId { get; set; }

        public string XPath { get; set; }

        public bool IsRequired { get; set; }

        public bool IsReviewNeeded { get; set; }

        public InitiationDto Initiation { get; set; }

        public static CollectionStepRequestedEventArgs Create(Guid collectionStepId,
                                                              Guid userId,
                                                              string xpath,
                                                              bool isRequired,
                                                              bool isReviewNeeded,
                                                              InitiationDto initiation) =>
            new CollectionStepRequestedEventArgs
            {
                CollectionStepId = collectionStepId,
                UserId = userId,
                XPath = xpath,
                IsRequired = isRequired,
                IsReviewNeeded = isReviewNeeded,
                Initiation = initiation
            };
    }
}