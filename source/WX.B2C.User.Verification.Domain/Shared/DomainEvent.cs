using System;

namespace WX.B2C.User.Verification.Domain.Shared
{
    public abstract class DomainEvent
    {
        protected DomainEvent()
        {
            CreationDate = DateTime.UtcNow;
        }

        public DateTime CreationDate { get; }
    }
}
