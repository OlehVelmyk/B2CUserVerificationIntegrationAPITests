using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.Api.Internal.Client;
using WX.B2C.User.Verification.Component.Tests.Constants;
using IInternalApiClientFactory = WX.B2C.User.Verification.Api.Internal.Client.IUserVerificationApiClientFactory;

namespace WX.B2C.User.Verification.Component.Tests.Internal
{
    internal class LookupTests : BaseComponentTest
    {
        private IInternalApiClientFactory _internalApiClientFactory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _internalApiClientFactory = Resolve<IInternalApiClientFactory>();
        }
        
        /// <summary>
        /// When client request countries infos
        /// Then he receives it
        /// And it contains country region
        /// And it contains phone code
        /// And it contains states info
        /// </summary>
        [Theory]
        public async Task ShouldGetCountriesInfo()
        {
            // Arrange
            var client = _internalApiClientFactory.Create(Guid.NewGuid());
            
            // Act
            var countryOptions = await client.Lookup.GetCountriesAsync();
            
            // Assert
            countryOptions.Should().NotBeEmpty();
            foreach (var country in countryOptions)
            {
                country.Name.Should().NotBeNull();
                country.PhoneCode.Should().NotBeEmpty();
                country.Alpha2Code.Should().NotBeNull();
                country.Alpha3Code.Should().NotBeNull();
                if (!country.IsNotSupported)
                {
                    country.Region.Should().NotBeNull();
                    CountryCodes.Supported.Should().Contain(country.Alpha2Code);
                }

                if (country.IsStateRequired)
                    country.States.Should().NotBeEmpty();
            }
        }
    }
}
