using System;
using System.Net.Mail;
using FsCheck;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries.Generators;

namespace WX.B2C.User.Verification.Unit.Tests.Arbitraries
{
    public class NotEmptyPersonalDetailsArbitrary : Arbitrary<NotEmptyPersonalDetails>
    {
        public static Arbitrary<NotEmptyPersonalDetails> Create()
        {
            return new NotEmptyPersonalDetailsArbitrary();
        }

        public override Gen<NotEmptyPersonalDetails> Generator =>
            from userId in Arb.Generate<Guid>()
            from email in Arb.Generate<MailAddress>()
            from birthDate in Arb.Generate<DateTime>()
            from nationality in CountryCodeGenerators.Countries()
            from firstName in StringGenerators.LettersOnly(6, 10)
            from lastName in StringGenerators.LettersOnly(6, 10)
            from address in Arb.Generate<AddressDto>()
            select new NotEmptyPersonalDetails()
            {
                UserId = userId,
                Email = email.Address,
                DateOfBirth = birthDate,
                Nationality = nationality,
                FirstName = firstName,
                LastName = lastName,
                ResidenceAddress = new AddressDto()
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

    public class NotEmptyPersonalDetails
    {
        public Guid UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public AddressDto ResidenceAddress { get; set; }

        public string Nationality { get; set; }

        public string Email { get; set; }

        public static implicit operator PersonalDetailsDto(NotEmptyPersonalDetails notEmptyPersonalDetails) =>
            new PersonalDetailsDto()
            {
                DateOfBirth = notEmptyPersonalDetails.DateOfBirth,
                Email = notEmptyPersonalDetails.Email,
                FirstName = notEmptyPersonalDetails.FirstName,
                LastName = notEmptyPersonalDetails.LastName,
                Nationality = notEmptyPersonalDetails.Nationality,
                ResidenceAddress = notEmptyPersonalDetails.ResidenceAddress,
                UserId = notEmptyPersonalDetails.UserId,
            };
    }
}