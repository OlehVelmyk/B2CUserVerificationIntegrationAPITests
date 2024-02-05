using System;
using WX.B2C.User.Verification.Events.Internal.Dtos;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class CollectionStepRequiredEventArgs : System.EventArgs
    {
        public Guid CollectionStepId { get; set; }

        public Guid UserId { get; set; }

        public InitiationDto Initiation { get; set; }

        public static CollectionStepRequiredEventArgs Create(Guid collectionStepId,
                                                             Guid userId,
                                                             InitiationDto initiation) =>
            new CollectionStepRequiredEventArgs
            {
                CollectionStepId = collectionStepId,
                UserId = userId,
                Initiation = initiation
            };
    }
}