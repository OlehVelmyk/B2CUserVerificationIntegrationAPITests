using FsCheck;
using WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries
{
    internal class TinArbitrary : Arbitrary<Tin>
    {
        public static Arbitrary<Tin> Create() => new TinArbitrary();

        public override Gen<Tin> Generator =>
            from type in Arb.Generate<TinType>()
            from number in TinGenerator.GenerateNumber(type)
            select new Tin
            {
                Type = type,
                Number = number
            };
    }

    internal class InvalidTinArbitrary : Arbitrary<InvalidTin>
    {
        public static Arbitrary<InvalidTin> Create() => new InvalidTinArbitrary();

        public override Gen<InvalidTin> Generator =>
            from type in Arb.Generate<TinType>()
            from number in TinGenerator.GenerateInvalidNumber()
            select new InvalidTin
            {
                Type = type,
                Number = number
            };
    }
}