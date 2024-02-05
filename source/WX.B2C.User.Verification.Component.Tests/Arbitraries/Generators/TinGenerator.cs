using System;
using FsCheck;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators
{
    internal static class TinGenerator
    {
        public static Gen<string> GenerateNumber(TinType tinType) =>
            tinType switch
            {
                TinType.SSN  => SsnNumber(),
                TinType.ITIN => ItinNumber(),
                _            => throw new ArgumentOutOfRangeException(nameof(tinType), tinType, null)
            };


        public static Gen<string> GenerateInvalidNumber() =>
            from number in StringGenerators.Numbers(6, 6)
            select "000" + number;

        private static Gen<string> ItinNumber() =>
            from partOne in Gen.Choose(0, 99)
            from partTwo in Gen.OneOf(Gen.Choose(50, 59),
                                      Gen.Choose(60, 65),
                                      Gen.Choose(83, 88),
                                      Gen.Choose(90, 92),
                                      Gen.Choose(94, 99))
            from partThree in Gen.Choose(0, 9999)
            select "9" +
                   partOne.ToString().PadLeft(2, '0') +
                   partTwo +
                   partThree.ToString().PadLeft(4, '0');

        private static Gen<string> SsnNumber() =>
            from partOne in Gen.Choose(1, 899)
            where partOne != 666
            from partTwo in Gen.Choose(1, 99)
            from partThree in Gen.Choose(1, 9999)
            select partOne.ToString().PadLeft(3, '0') +
                   partTwo.ToString().PadLeft(2, '0') +
                   partThree.ToString().PadLeft(4, '0');
    }
}