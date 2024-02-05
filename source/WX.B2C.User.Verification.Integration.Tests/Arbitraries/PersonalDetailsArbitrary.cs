using System;
using System.Net.Mail;
using FsCheck;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.Generators;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries
{
    internal class PersonalDetailsArbitrary : Arbitrary<PersonalDetailsDto>
    {
        public static Arbitrary<PersonalDetailsDto> Create() => new PersonalDetailsArbitrary();

        public override Gen<PersonalDetailsDto> Generator =>
            from userId in Arb.Generate<Guid>()
            from email in Arb.Generate<MailAddress>()
            from birthDate in Arb.Generate<DateTime?>()
            from nationality in CountryCodeGenerators.Countries()
            from firstName in StringGenerators.LettersOnly(6,10)
            from lastName in StringGenerators.LettersOnly(6,10)
            from seed in Gen.Choose(int.MinValue, int.MaxValue)
            from address in AddressGenerator.Address(seed)
            select new PersonalDetailsDto
            {
                UserId = userId,
                Email = email.Address,
                DateOfBirth = birthDate,
                Nationality = nationality,
                FirstName = firstName,
                LastName = lastName,
                ResidenceAddress = address
            };
    }
}