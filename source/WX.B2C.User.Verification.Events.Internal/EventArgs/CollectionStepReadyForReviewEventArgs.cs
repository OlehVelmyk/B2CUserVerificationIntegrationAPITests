using System;
using WX.B2C.User.Verification.Events.Internal.Dtos;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class CollectionStepReadyForReviewEventArgs : System.EventArgs
    {
        public Guid CollectionStepId { get; set; }

        public Guid UserId { get; set; }

        public string XPath { get; set; }

        public InitiationDto Initiation { get; set; }

        public static CollectionStepReadyForReviewEventArgs Create(Guid collectionStepId,
                                                              Guid userId,
                                                              string xPath,
                                                              InitiationDto initiation) =>
            new CollectionStepReadyForReviewEventArgs
            {
                CollectionStepId = collectionStepId,
                UserId = userId,
                XPath = xPath,
                Initiation = initiation
            };
    }
}
