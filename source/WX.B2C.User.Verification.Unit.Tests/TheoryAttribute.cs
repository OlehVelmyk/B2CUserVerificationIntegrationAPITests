using WX.B2C.User.Verification.Unit.Tests.Arbitraries;

namespace WX.B2C.User.Verification.Unit.Tests
{
    public class TheoryAttribute : FsCheck.NUnit.PropertyAttribute
    {
        public TheoryAttribute()
        {
            MaxTest = 1;
            QuietOnSuccess = true;
            Arbitrary = new[]
            {
                typeof(VerificationDetailsArbitrary),
                typeof(NotEmptyVerificationDetailsArbitrary),
                typeof(TaxResidenceArbitrary),
                typeof(ResidenceAddressArbitrary),
                typeof(IpAddressLocationArbitrary),
                typeof(TinArbitrary),
                typeof(PersonalDetailsArbitrary),
                typeof(NotEmptyPersonalDetailsArbitrary),
                typeof(XPathArbitrary),
                typeof(TicketInfoArbitrary)
            };
        }
    }
}
