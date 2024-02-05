using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers
{
    public interface IPersonalDetailsMapper
    {
        Dtos.PersonalDetailsDto SafeMap(PersonalDetailsDto personalDetails);
    }

    internal class PersonalDetailsMapper : IPersonalDetailsMapper
    {
        public Dtos.PersonalDetailsDto SafeMap(PersonalDetailsDto personalDetails)
        {
            if (personalDetails == null) return null;

            return new Dtos.PersonalDetailsDto
            {
                FirstName = personalDetails.FirstName,
                LastName = personalDetails.LastName,
                DateOfBirth = personalDetails.DateOfBirth,
                Email = personalDetails.Email,
                Nationality = personalDetails.Nationality,
                ResidenceAddress = SafeMap(personalDetails.ResidenceAddress)
            };
        }

        public Dtos.AddressDto SafeMap(AddressDto address)
        {
            if (address == null) return null;

            return new Dtos.AddressDto
            {
                Line1 = address.Line1,
                Line2 = address.Line2,
                City = address.City,
                Country = address.Country,
                State = address.State,
                ZipCode = address.ZipCode,
            };
        }
    }
}
