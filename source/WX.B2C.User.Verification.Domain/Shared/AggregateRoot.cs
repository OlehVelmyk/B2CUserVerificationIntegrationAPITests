using System;
using System.Collections.Generic;
using System.Linq;

namespace WX.B2C.User.Verification.Domain.Shared
{
    public abstract class AggregateRoot : AggregateRoot<Guid>
    {
        protected AggregateRoot(Guid id)
            : base(id)
        {
        }
    }

    public abstract class AggregateRoot<TKey> where TKey : IComparable<TKey>, IEquatable<TKey>
    {
        private readonly ICollection<DomainEvent> _uncommittedEvents;

        protected AggregateRoot(TKey id, int version = 0)
        {
            if (id == null || id.Equals(default))
                throw new ArgumentException(nameof(id));

            if (version < 0)
                throw new ArgumentException(nameof(version));

            Id = id;
            Version = version;
            _uncommittedEvents = new HashSet<DomainEvent>();
        }

        public TKey Id { get; }

        public int Version { get; private set; }

        public bool HasChanges => _uncommittedEvents.Count > 0;

        protected void Apply(DomainEvent @event)
        {
            Version++;
            _uncommittedEvents.Add(@event);
        }

        public DomainEvent[] CommitEvents()
        {
            var events = _uncommittedEvents
                         .OrderBy(e => e.CreationDate)
                         .ToArray();

            _uncommittedEvents.Clear();
            return events;
        }
    }
}
