using System;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Events
{
    public class CheckCreated : DomainEvent
    {
        public Guid Id { get; private set; }

        public Guid VariantId { get; private set; }

        public Guid UserId { get; private set; }

        public Guid[] RelatedTasks { get; private set; }

        public Initiation Initiation { get; private set; }

        public static CheckCreated Create(Check check, Initiation initiation)
        {
            if (check == null)
                throw new ArgumentNullException(nameof(check));
            if (initiation == null)
                throw new ArgumentNullException(nameof(initiation));

            return new CheckCreated
            {
                Id = check.Id,
                VariantId = check.Variant.Id,
                UserId = check.UserId,
                RelatedTasks = check.RelatedTasks,
                Initiation = initiation
            };
        }
    }
}