using System;
using System.Net.Mail;
using FsCheck;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries.Generators;

namespace WX.B2C.User.Verification.Unit.Tests.Arbitraries
{
    public class PersonalDetailsArbitrary : Arbitrary<PersonalDetailsDto>
    {
        public static Arbitrary<PersonalDetailsDto> Create() => new PersonalDetailsArbitrary();

        public override Gen<PersonalDetailsDto> Generator =>
            from userId in Arb.Generate<Guid>()
            from email in Arb.Generate<MailAddress>()
            from birthDate in Arb.Generate<DateTime>()
            from nationality in CountryCodeGenerators.Countries()
            from firstName in StringGenerators.LettersOnly(6, 10)
            from lastName in StringGenerators.LettersOnly(6, 10)
            from address in Arb.Generate<AddressDto>()
            select new PersonalDetailsDto
            {
                UserId = userId,
                Email = email.Address,
                DateOfBirth = birthDate,
                Nationality = nationality,
                FirstName = firstName,
                LastName = lastName,
                ResidenceAddress = new AddressDto
                {
                    State = address.State,
                    City = address.City,
                    Country = address.Country,
                    Line1 = address.Line1,
                    Line2 = address.Line2,
                    ZipCode = address.ZipCode
                }
            };
    }
}