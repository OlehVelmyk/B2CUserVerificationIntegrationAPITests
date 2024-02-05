using System;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class TaskCollectionStepRemovedEventArgs : System.EventArgs
    {
        public Guid TaskId { get; set; }
        
        public Guid UserId { get; set; }
        
        public Guid[] CollectionStepIds { get; set; }
        
        public static TaskCollectionStepRemovedEventArgs Create(Guid taskId, Guid userId, Guid[] collectionStepIds) =>
            new TaskCollectionStepRemovedEventArgs
            {
                TaskId = taskId,
                UserId = userId,
                CollectionStepIds = collectionStepIds
            };
    }
}