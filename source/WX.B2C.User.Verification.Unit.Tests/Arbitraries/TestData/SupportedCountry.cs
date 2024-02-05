using System;
using System.Diagnostics.CodeAnalysis;

namespace WX.B2C.User.Verification.Unit.Tests.Arbitraries.TestData
{
    internal class SupportedCountry : IEquatable<SupportedCountry>
    {
        public string Value { get; set; }

        public bool Equals([AllowNull] SupportedCountry other) => 
            other is not null && other.Value == Value;

        public override int GetHashCode() => Value.GetHashCode();

        public static implicit operator string(SupportedCountry country) => country.Value;
    }
}
