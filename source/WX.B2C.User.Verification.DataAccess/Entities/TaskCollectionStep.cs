using System;

namespace WX.B2C.User.Verification.DataAccess.Entities
{
    internal class TaskCollectionStep
    {
        public Guid TaskId { get; set; }

        public Guid StepId { get; set; }

        public virtual CollectionStep Step { get; set; }
    }
}