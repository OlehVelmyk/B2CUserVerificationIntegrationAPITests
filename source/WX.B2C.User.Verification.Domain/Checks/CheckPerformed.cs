using System;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Events
{
    public class CheckPerformed : DomainEvent
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public Guid VariantId { get; private set; }

        public static CheckPerformed Create(Check check)
        {
            if (check == null)
                throw new ArgumentNullException(nameof(check));

            return new CheckPerformed
            {
                Id = check.Id,
                UserId = check.UserId,
                VariantId = check.Variant.Id
            };
        }
    }
}