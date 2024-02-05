using System;
using System.Collections.Generic;
using FsCheck;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.PassFort.Client.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries.ExternalProviders
{
    internal class PassFortProfileArbitrary :  Arbitrary<PassFortProfile>
    {
        public static Arbitrary<PassFortProfile> Create() => new PassFortProfileArbitrary();

        public override Gen<PassFortProfile> Generator =>
            from seed in Arb.Generate<int>()
            from birthDate in Arb.Generate<DateTime>()
            from firstName in PersonalDetailsGenerator.FirstName(seed)
            from lastName in PersonalDetailsGenerator.LastName(seed)
            from email in PersonalDetailsGenerator.Email(seed)
            select new PassFortProfile
            {
                Role = "INDIVIDUAL_CUSTOMER",
                CollectedData = new IndividualData
                {
                    EntityType = "INDIVIDUAL",
                    PersonalDetails = new PersonalDetails
                    {
                        Dob = birthDate.ToString("yyyy-MM-dd"),
                        Name = new FullName
                        {
                            FamilyName = lastName,
                            GivenNames = new List<string> { firstName }
                        }
                    },
                    ContactDetails = new IndividualDataContactDetails
                    {
                        Email = email
                    }
                }
            };
    }
}
