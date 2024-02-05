using System;
using FsCheck;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries.Generators
{
    internal static class DateTimeGenerators
    {
        public static Gen<DateTime> UtcNow => Gen.Constant(DateTime.UtcNow);

        public static Gen<DateTime> BeforeUtcNow(int offsetInYears = 0) => 
            Arb.Generate<DateTime>()
               .Where(d => d.CompareTo(DateTime.UtcNow.AddYears(offsetInYears)) == -1);

        public static Gen<DateTime> AfterUtcNow(int offsetInYears = 0) =>
            Arb.Generate<DateTime>()
               .Where(d => d.CompareTo(DateTime.UtcNow.AddYears(offsetInYears)) == 1);
    }
}
