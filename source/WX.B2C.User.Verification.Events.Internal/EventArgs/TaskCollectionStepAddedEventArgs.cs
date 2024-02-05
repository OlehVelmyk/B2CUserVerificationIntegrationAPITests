using System;
using WX.B2C.User.Verification.Events.Internal.Dtos;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class TaskCollectionStepAddedEventArgs : System.EventArgs
    {
        public Guid TaskId { get; set; }
        
        public Guid UserId { get; set; }
        
        public Guid CollectionStepId { get; set; }

        public bool IsRequired { get; set; }

        public InitiationDto Initiation { get; set; }

        public static TaskCollectionStepAddedEventArgs Create(Guid taskId, 
                                                              Guid userId, 
                                                              Guid collectionStepId, 
                                                              bool isRequired, 
                                                              InitiationDto initiation) =>
            new TaskCollectionStepAddedEventArgs
            {
                TaskId = taskId,
                UserId = userId,
                CollectionStepId = collectionStepId,
                IsRequired = isRequired,
                Initiation = initiation
            };
    }
}