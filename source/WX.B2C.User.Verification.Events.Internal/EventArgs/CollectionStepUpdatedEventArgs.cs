using System;
using WX.B2C.User.Verification.Events.Internal.Dtos;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class CollectionStepUpdatedEventArgs : System.EventArgs
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public bool IsRequired { get; set; }

        public bool IsReviewNeeded { get; set; }

        public InitiationDto Initiation { get; set; }

        public static CollectionStepUpdatedEventArgs Create(Guid id, 
                                                            Guid userId, 
                                                            bool isRequired, 
                                                            bool isReviewNeeded, 
                                                            InitiationDto initiationDto) =>
            new CollectionStepUpdatedEventArgs
            {
                Id = id,
                UserId = userId,
                IsRequired = isRequired,
                IsReviewNeeded = isReviewNeeded,
                Initiation = initiationDto
            };
    }
}
