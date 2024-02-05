using WX.B2C.User.Profile.Events.Dtos;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Extensions
{
    internal static class AddressMapperExtensions
    {
        public static AddressDto SafeMap(this Address address)
        {
            if (address is null)
                return null;

            return new AddressDto
            {
                StateCode = address.State,
                City = address.City,
                Country = address.Country,
                Line1 = address.Line1,
                Line2 = address.Line2,
                ZipCode = address.ZipCode,
            };
        }
    }
}