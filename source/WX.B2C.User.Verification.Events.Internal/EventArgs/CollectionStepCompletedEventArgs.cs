using System;
using WX.B2C.User.Verification.Events.Internal.Dtos;
using WX.B2C.User.Verification.Events.Internal.Enums;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class CollectionStepCompletedEventArgs : System.EventArgs
    {
        public Guid UserId { get; set; }

        public Guid CollectionStepId { get; set; }

        public string XPath { get; set; }
        
        public CollectionStepReviewResult? ReviewResult { get; set; }

        public InitiationDto Initiation { get; set; }

        public static CollectionStepCompletedEventArgs Create(Guid collectionStepId,
                                                              Guid userId,
                                                              string xPath, 
                                                              CollectionStepReviewResult? reviewResult,
                                                              InitiationDto initiation) =>
            new CollectionStepCompletedEventArgs
            {
                CollectionStepId = collectionStepId,
                UserId = userId,
                XPath = xPath,
                ReviewResult = reviewResult,
                Initiation = initiation
            };
    }
}
