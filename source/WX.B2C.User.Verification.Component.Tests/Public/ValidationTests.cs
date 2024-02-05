using System;
using System.Linq;
using System.Linq.Expressions;
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

namespace WX.B2C.User.Verification.Component.Tests.Public
{
    internal class ValidationTests : BaseComponentTest
    {
        private ApplicationFixture _applicationFixture;
        private ProfileFixture _profileFixture;
        private PublicApiClientFactory _publicApiClientFactory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _applicationFixture = Resolve<ApplicationFixture>();
            _profileFixture = Resolve<ProfileFixture>();
            _publicApiClientFactory = Resolve<PublicApiClientFactory>();

            Arb.Register<UserInfoArbitrary>();
            Arb.Register<UsUserInfoArbitrary>();
            Arb.Register<NotUsUserInfoArbitrary>();
            Arb.Register<UnsupportedUserInfoArbitrary>();
        }

        /// <summary>
        /// Scenario: Get all validation rules (US)
        /// Given user from US with personal details 
        /// And with residence country
        /// When he requests all validation rules
        /// Then he receives verification details and document rules
        /// And rules contain validation details 
        /// </summary>
        [Theory]
        public async Task ShoulGetValidationRulesForUs(UsUserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress);

            await _applicationFixture.RegisterApplicationAsync(userInfo);

            // Arrange
            var expectedCategories = new[] { DocumentCategory.ProofOfIdentity, DocumentCategory.ProofOfAddress, DocumentCategory.ProofOfFunds, DocumentCategory.Selfie };

            // Act
            var rules = await publicClient.Validation.GetRulesAsync();

            // Assert
            rules.Should().NotBeNull();
            var verificationDetailsRule = rules.VerificationDetailsRule;
            var documentRules = rules.DocumentRules;

            verificationDetailsRule.Should().NotBeNull();
            verificationDetailsRule.TaxResidences.Should().NotBeNullOrEmpty();
            verificationDetailsRule.TinValidationRules.Should().NotBeNullOrEmpty();
            foreach (var tinRule in verificationDetailsRule.TinValidationRules)
            {
                tinRule.Should().NotBeNull();
                tinRule.TinType.Should().HaveValue();
                tinRule.ValidationRegex.Should().NotBeEmpty();
            }

            documentRules.Should().NotBeNullOrEmpty();
            documentRules.Select(rule => rule.DocumentCategory).Should().BeEquivalentTo(expectedCategories);
            foreach(var documentRule in documentRules)
            {
                documentRule.Should().NotBeNull();
                documentRule.DocumentCategory.Should().HaveValue();

                var isContainsFileCount = GetIsContainsFileCountExpression();
                foreach (var rule in documentRule.ValidationRules)
                {
                    rule.Should().NotBeNull();
                    rule.DocumentType.Should().NotBeNull();
                    rule.DescriptionCode.Should().NotBeNull();
                    rule.MaxSizeInBytes.Should().BeGreaterThan(0);
                    rule.Extensions.Should().NotBeNullOrEmpty();
                    rule.Should().Match(isContainsFileCount);
                }
            }

            static Expression<Func<DocumentValidationRuleItemDto, bool>> GetIsContainsFileCountExpression() =>
                rule => rule.DocumentSide != null || (rule.MinQuantity != null && rule.MaxQuantity != null);
        }
        
        /// <summary>
        /// Scenario: Get all validation rules (Not US)
        /// Given user not from US with personal details 
        /// And with residence country
        /// When he requests all validation rules
        /// Then he receives verification details and document rules
        /// And rules contain validation details 
        /// </summary>
        [Theory]
        public async Task ShoulGetValidationRules(NotUsUserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress);

            await _applicationFixture.RegisterApplicationAsync(userInfo);

            // Arrange
            var expectedCategories = new[] { DocumentCategory.ProofOfIdentity, DocumentCategory.ProofOfAddress, 
                                             DocumentCategory.ProofOfFunds, DocumentCategory.Selfie, DocumentCategory.Taxation };

            // Act
            var rules = await publicClient.Validation.GetRulesAsync();

            // Assert
            rules.Should().NotBeNull();
            var verificationDetailsRule = rules.VerificationDetailsRule;
            var documentRules = rules.DocumentRules;

            verificationDetailsRule.Should().NotBeNull();
            verificationDetailsRule.TaxResidences.Should().NotBeNullOrEmpty();

            documentRules.Should().NotBeNullOrEmpty();
            documentRules.Select(rule => rule.DocumentCategory).Should().BeEquivalentTo(expectedCategories);
            foreach(var documentRule in documentRules)
            {
                documentRule.Should().NotBeNull();
                documentRule.DocumentCategory.Should().HaveValue();

                var isContainsFileCount = GetIsContainsFileCountExpression();
                foreach (var rule in documentRule.ValidationRules)
                {
                    rule.Should().NotBeNull();
                    rule.DocumentType.Should().NotBeNull();
                    rule.DescriptionCode.Should().NotBeNull();
                    rule.MaxSizeInBytes.Should().BeGreaterThan(0);
                    rule.Extensions.Should().NotBeNullOrEmpty();
                    rule.Should().Match(isContainsFileCount);
                }
            }

            static Expression<Func<DocumentValidationRuleItemDto, bool>> GetIsContainsFileCountExpression() =>
                rule => rule.DocumentSide != null || (rule.MinQuantity != null && rule.MaxQuantity != null);
        }

        /// <summary>
        /// Scenario: Get all validation rules
        /// Given user with personal details 
        /// And without residence county
        /// When he requests all validation rules
        /// Then he receives error response with status code "Too Early"
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenUserWithoutCountry(UsUserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            userInfo.Address = null;

            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress);
            await _profileFixture.CreateAsync(userInfo);

            // Act
            Func<Task> getRules = () => publicClient.Validation.GetRulesAsync();

            // Assert
            var exception = await getRules.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().HaveValue(425);
        }
        
        /// <summary>
        /// Scenario: Get all validation rules (Unsupported)
        /// Given user from unsupported country
        /// And without application 
        /// When he requests all validation rules
        /// Then he receives error response with status code "Internal server error"
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenUserFromUnsupportedCountry(UnsupportedUserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress);
            await _profileFixture.CreateAsync(userInfo);

            // Act
            Func<Task> getRules = () => publicClient.Validation.GetRulesAsync();

            // Assert
            var exception = await getRules.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// Scenario: Get verification details rule (US)
        /// Given user from US with personal details 
        /// And with residence country
        /// When he requests verification details rule
        /// Then he receives rules about 'TIN' and 'TaxResidence'
        /// And rules contain allowed 'TIN' types and countries 
        /// </summary>
        [Theory]
        public async Task ShoulGetVerificationDetailsRuleForUs(UsUserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress);

            await _applicationFixture.RegisterApplicationAsync(userInfo);

            // Act
            var verificationDetailsRule = await publicClient.Validation.GetVerificationDetailsRulesAsync();

            // Assert
            verificationDetailsRule.Should().NotBeNull();
            verificationDetailsRule.TaxResidences.Should().NotBeNullOrEmpty();
            verificationDetailsRule.TinValidationRules.Should().NotBeNullOrEmpty();
            foreach(var tinRule in verificationDetailsRule.TinValidationRules)
            {
                tinRule.Should().NotBeNull();
                tinRule.TinType.Should().HaveValue();
                tinRule.ValidationRegex.Should().NotBeEmpty();
            }
        }

        /// <summary>
        /// Scenario: Get verification details rule (Not US)
        /// Given user not from US with personal details 
        /// And with residence country
        /// When he requests verification details rule
        /// Then he receives rule about 'TaxResidence'
        /// And rule contains allowed countries 
        /// </summary>
        [Theory]
        public async Task ShoulGetVerificationDetailsRule(NotUsUserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress);

            await _applicationFixture.RegisterApplicationAsync(userInfo);

            // Act
            var verificationDetailsRule = await publicClient.Validation.GetVerificationDetailsRulesAsync();

            // Assert
            verificationDetailsRule.Should().NotBeNull();
            verificationDetailsRule.TaxResidences.Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Scenario: Get document rules for ProofOfIdentity
        /// Given user with personal details 
        /// And with residence country
        /// When he requests document rule for ProofOfIdentity
        /// Then he receives rule about ProofOfIdentity
        /// And rule contains allowed document types 
        /// And every type contains validation details
        /// </summary>
        [Theory]
        public Task ShoulGetDocumentRuleForPoI(UserInfo userInfo) =>
            ShoulGetDocumentRule(userInfo, DocumentCategory.ProofOfIdentity);

        /// <summary>
        /// Scenario: Get document rules for ProofOfAddress
        /// Given user with personal details 
        /// And with residence country
        /// When he requests document rule for ProofOfAddress
        /// Then he receives rule about ProofOfAddress
        /// And rule contains allowed document types 
        /// And every type contains validation details
        /// </summary>
        [Theory]
        public Task ShoulGetDocumentRuleForPoA(UserInfo userInfo) =>
            ShoulGetDocumentRule(userInfo, DocumentCategory.ProofOfAddress);

        /// <summary>
        /// Scenario: Get document rules for ProofOfFunds
        /// Given user with personal details 
        /// And with residence country
        /// When he requests document rule for ProofOfFunds
        /// Then he receives rule about ProofOfFunds
        /// And rule contains allowed document types 
        /// And every type contains validation details
        /// </summary>
        [Theory]
        public Task ShoulGetDocumentRuleForPoF(UserInfo userInfo) =>
            ShoulGetDocumentRule(userInfo, DocumentCategory.ProofOfFunds);

        /// <summary>
        /// Scenario: Get document rules for Selfie
        /// Given user with personal details 
        /// And with residence country
        /// When he requests document rule for Selfie
        /// Then he receives rule about Selfie
        /// And rule contains allowed document types 
        /// And every type contains validation details
        /// </summary>
        [Theory]
        public Task ShoulGetDocumentRuleForSelfie(UserInfo userInfo) =>
            ShoulGetDocumentRule(userInfo, DocumentCategory.Selfie);

        /// <summary>
        /// Scenario: Get document rules for Taxation
        /// Given user with personal details 
        /// And with residence country
        /// When he requests document rule for Taxation
        /// Then he receives rule about Taxation
        /// And rule contains allowed document types 
        /// And every type contains validation details
        /// </summary>
        [Theory]
        public Task ShoulGetDocumentRuleForTaxation(NotUsUserInfo userInfo) =>
            ShoulGetDocumentRule(userInfo, DocumentCategory.Taxation);

        private async Task ShoulGetDocumentRule(UserInfo userInfo, DocumentCategory documentCategory)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress);

            await _applicationFixture.RegisterApplicationAsync(userInfo);

            // Act
            var documentRule = await publicClient.Validation.GetDocumentRulesAsync(documentCategory);

            // Assert
            documentRule.Should().NotBeNull();
            documentRule.DocumentCategory.Should().Be(documentCategory);

            var isContainsFileCount = GetIsContainsFileCountExpression();
            foreach (var rule in documentRule.ValidationRules)
            {
                rule.Should().NotBeNull();
                rule.DocumentType.Should().NotBeNull();
                rule.DescriptionCode.Should().NotBeNull();
                rule.MaxSizeInBytes.Should().BeGreaterThan(0);
                rule.Extensions.Should().NotBeNullOrEmpty();
                rule.Should().Match(isContainsFileCount);
            }

            static Expression<Func<DocumentValidationRuleItemDto, bool>> GetIsContainsFileCountExpression() =>
                rule => rule.DocumentSide != null || (rule.MinQuantity != null && rule.MaxQuantity != null);
        }

        /// <summary>
        /// Scenario: Get document rules (Invalid document category)
        /// Given user with personal details 
        /// And with residence country
        /// When he requests document rule for document category
        /// And category is cannot be mapped to action
        /// Then he receives error response with status code "Unprocessable Entity"
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenInvalidDocumentCategory(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress);

            await _applicationFixture.RegisterApplicationAsync(userInfo);

            // Act
            Func<Task> getRules = () => publicClient.Validation.GetDocumentRulesAsync(DocumentCategory.Supporting);

            // Assert
            var exception = await getRules.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        }
    }
}
