using System;
using Optional;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;

namespace WX.B2C.User.Verification.Facade.EventHandlers.Mappers
{
    internal interface IProfileDetailsMapper
    {
        PersonalDetailsPatch Map(Profile.Events.Dtos.UserProfileDto source);
    }

    internal class ProfileDetailsMapper: IProfileDetailsMapper
    {
        public PersonalDetailsPatch Map(Profile.Events.Dtos.UserProfileDto source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new PersonalDetailsPatch
            {
                FirstName = source.FirstName.SomeNotNull(),
                LastName = source.LastName.SomeNotNull(),
                DateOfBirth = source.DateOfBirth.SomeNotNull(),
                Nationality = source.Nationality.SomeNotNull(),
                Email = source.Email.SomeNotNull(),
                ResidenceAddress = source.ResidenceAddress.SomeNotNull().Map(Map)
            };
        }

        private static AddressDto Map(Profile.Events.Dtos.AddressDto source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new AddressDto
            {
                Line1 = source.Line1,
                Line2 = source.Line2,
                City = source.City,
                State = source.StateCode,
                Country = source.Country,
                ZipCode = source.ZipCode
            };
        }
    }
}