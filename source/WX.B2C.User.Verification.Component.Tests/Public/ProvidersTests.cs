using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Api.Public.Client;
using WX.B2C.User.Verification.Api.Public.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Arbitraries;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Component.Tests.Mountebank;

namespace WX.B2C.User.Verification.Component.Tests.Public
{
    internal class ProvidersTests : BaseComponentTest
    {
        private ProfileFixture _profileFixture;
        private PublicApiClientFactory _publicApiClientFactory;
        private IOnfidoImposter _onfidoImposter;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _profileFixture = Resolve<ProfileFixture>();
            _publicApiClientFactory = Resolve<PublicApiClientFactory>();
            _onfidoImposter = Resolve<IOnfidoImposter>();

            Arb.Register<SeedArbitrary>();
            Arb.Register<UserInfoArbitrary>();
        }

        /// <summary>
        /// Scenario: User gets Onfido sdk-token (Web)
        /// Given user without verification application
        /// When user requests Onfido sdk-token
        /// And token type is 'Web'
        /// Then he receives it
        /// </summary>
        [Theory]
        public async Task ShouldGetSdkTokenForOnfido_WhenTokenTypeWeb(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var client = _publicApiClientFactory.Create(userId, userInfo.IpAddress);

            await _profileFixture.CreateAsync(userInfo);

            // Arrange
            var request = new SdkTokenRequest(TokenType.Web);

            // Act
            var sdkToken = await client.Providers.PostAsync(request);

            // Assert
            sdkToken.Should().NotBeNull();
            sdkToken.ApplicantId.Should().NotBeNullOrWhiteSpace().And.NotBe(Guid.Empty.ToString());
            sdkToken.Token.Should().NotBeNullOrWhiteSpace();
        }

        /// <summary>
        /// Scenario: User gets Onfido sdk-token (Application)
        /// Given user without verification application
        /// When user requests Onfido sdk-token
        /// And token type is 'Application'
        /// Then he receives it
        /// </summary>
        [Theory]
        public async Task ShouldGetSdkTokenForOnfido_WhenTokenTypeApplication(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var client = _publicApiClientFactory.Create(userId, userInfo.IpAddress);

            await _profileFixture.CreateAsync(userInfo);

            // Arrange
            var request = new SdkTokenRequest(TokenType.Application, Guid.NewGuid().ToString());

            // Act
            var sdkToken = await client.Providers.PostAsync(request);

            // Assert
            sdkToken.Should().NotBeNull();
            sdkToken.ApplicantId.Should().NotBeNullOrWhiteSpace().And.NotBe(Guid.Empty.ToString());
            sdkToken.Token.Should().NotBeNullOrWhiteSpace();
        }

        /// <summary>
        /// Scenario: User gets Onfido sdk-token (Application)
        /// Given user without verification application
        /// When user requests Onfido sdk-token
        /// And token type is 'Application'
        /// And application id is not specified
        /// Then he receives error response with status code "Bad Request"
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenNoApplicationId(UserInfo userInfo)
        {            
            // Given
            var userId = userInfo.UserId.Dump();
            var client = _publicApiClientFactory.Create(userId, userInfo.IpAddress);

            await _profileFixture.CreateAsync(userInfo);

            // Arrange
            var request = new SdkTokenRequest(TokenType.Application);

            // Act
            var getSdkToken = new Func<Task>(() => client.Providers.PostAsync(request));

            // Assert
            var exception = await getSdkToken.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Scenario: User gets not Onfido sdk-token
        /// Given user without verification application
        /// When user requests not Onfido sdk-token
        /// Then he receives error response with status code "Bad Request"
        /// </summary>
        [Test, Ignore("Now token provider is only Onfido")]
        public async Task ShouldGetError_WhenTokenProviderNotOnfido()
        {
        }
    }
}
