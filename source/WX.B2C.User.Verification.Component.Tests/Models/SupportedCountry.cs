using System;

namespace WX.B2C.User.Verification.Component.Tests.Models
{
    internal class SupportedCountry
    {
        public string Alpha2Code { get; }

        public SupportedCountry(string alpha2Code)
        {
            Alpha2Code = alpha2Code ?? throw new ArgumentNullException(nameof(alpha2Code));
        }

        public static explicit operator string(SupportedCountry country) =>
            country.Alpha2Code;
    }
}