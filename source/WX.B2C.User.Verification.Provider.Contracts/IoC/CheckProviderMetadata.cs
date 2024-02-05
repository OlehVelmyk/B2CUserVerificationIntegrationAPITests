using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Provider.Contracts.IoC
{
    public class CheckProviderMetadata : IEquatable<CheckProviderMetadata>
    {
        public CheckProviderMetadata(CheckType checkType, CheckProviderType providerType)
        {
            CheckType = checkType;
            ProviderType = providerType;
        }

        public CheckProviderMetadata(IDictionary<string, object> metadata)
        {
            ProviderType = (CheckProviderType)metadata[nameof(ProviderType)];
            if (metadata.ContainsKey(nameof(CheckType)))
                CheckType = (CheckType)metadata[nameof(CheckType)];
        }

        public CheckType? CheckType { get; }

        public CheckProviderType ProviderType { get; }

        public bool Equals(CheckProviderMetadata other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return CheckType == other.CheckType && ProviderType == other.ProviderType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((CheckProviderMetadata) obj);
        }

        public override int GetHashCode() => HashCode.Combine(CheckType.GetHashCode(), ProviderType.GetHashCode());
    }
}