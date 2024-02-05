using System.Linq;
using FsCheck;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries.Generators;

namespace WX.B2C.User.Verification.Unit.Tests.Arbitraries
{
    public class TinArbitrary : Arbitrary<TinDto>
    {
        public static Arbitrary<TinDto> Create()
        {
            return new TinArbitrary();
        }

        public override Gen<TinDto> Generator =>
            from number in Gen.ArrayOf(16,CharGenerators.Digits())
            select new TinDto { Number = new string(number), Type = number.Last() == '9' ? TinType.ITIN : TinType.SSN};
    }
}