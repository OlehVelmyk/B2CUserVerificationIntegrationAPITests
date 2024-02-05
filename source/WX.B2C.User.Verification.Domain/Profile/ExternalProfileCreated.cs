using System;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Events
{
    public class ExternalProfileCreated : DomainEvent
    {
        public Guid UserId { get; private set; }

        public ExternalProviderType ProviderType { get; private set; }

        public static ExternalProfileCreated Create(Guid userId, ExternalProviderType providerType) =>
            new()
            {
                UserId = userId,
                ProviderType = providerType
            };
    }
}