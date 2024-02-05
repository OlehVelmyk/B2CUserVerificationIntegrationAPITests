using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain
{
    public class CheckVariant : ValueObject
    {
        private CheckVariant(Guid id, CheckProviderType provider)
        {
            Id = id;
            Provider = provider;
        }

        public Guid Id { get; }

        public CheckProviderType Provider { get; }

        public static CheckVariant Create(Guid id, CheckProviderType provider) 
            => new (id, provider);

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Id;
            yield return Provider;
        }
    }
}