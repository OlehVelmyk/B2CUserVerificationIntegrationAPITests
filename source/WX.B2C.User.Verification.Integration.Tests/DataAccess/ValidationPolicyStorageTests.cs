using System;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using WX.B2C.User.Verification.Configuration.Seed;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Core.Services.Extensions;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Integration.Tests.DataAccess
{
    public class ValidationPolicyStorageTests : BaseIntegrationTest
    {
        [Test]
        public async Task ShouldReadPolicies_WhenCountryGB()
        {
            var expectedPoaDocumentTypes = new[]
            {
                AddressDocumentType.BankStatement.Value, 
                AddressDocumentType.UtilityBill.Value, 
                AddressDocumentType.CouncilTax.Value,
            };

            var storage = Resolve<IValidationPolicyStorage>();

            var policy = await storage.GetAsync(ValidationPolicySelectionContext.Create("GB", "Global"));

            policy.Should().NotBeNull();

            var taxResidenceValidationRule = policy.GetTaxResidenceValidationRule();
            taxResidenceValidationRule.Should().NotBeNull();
            taxResidenceValidationRule.AllowedCountries.Should().NotBeEmpty();

            var tinValidationRule = policy.GetTinValidationRule();
            tinValidationRule.Should().BeNull();

            var selfieRules = policy.GetDocumentValidationRule(ActionType.Selfie);
            selfieRules.Should().NotBeNull();
            selfieRules.AllowedTypes.Should().ContainSingle().Which.Key.Should().Be(SelfieDocumentType.Photo);

            var pofRules = policy.GetDocumentValidationRule(ActionType.ProofOfFunds);
            pofRules.Should().NotBeNull();
            pofRules.AllowedTypes.Should().NotBeNullOrEmpty();

            var w9FormRules = policy.GetDocumentValidationRule(ActionType.W9Form);
            w9FormRules.Should().NotBeNull();
            w9FormRules.AllowedTypes.Should().ContainSingle().Which.Key.Should().Be(TaxationDocumentType.W9Form);

            var poaRules = policy.GetDocumentValidationRule(ActionType.ProofOfAddress);
            poaRules.Should().NotBeNull();
            poaRules.AllowedTypes.Should().HaveCount(expectedPoaDocumentTypes.Length);
            poaRules.AllowedTypes.Select(dto => dto.Key).Should().BeEquivalentTo(expectedPoaDocumentTypes);
        }

        [Test]
        public async Task ShouldReadPolicies_WhenCountryUS()
        {
            var expectedPoaDocumentTypes = new[]
            {
                AddressDocumentType.BankStatement.Value,
                AddressDocumentType.UtilityBill.Value,
                AddressDocumentType.TaxReturn.Value,
                AddressDocumentType.CouncilTax.Value,
                AddressDocumentType.CertificateOfResidency.Value
            };

            var storage = Resolve<IValidationPolicyStorage>();

            var policy = await storage.GetAsync(ValidationPolicySelectionContext.Create("US", "Global"));

            policy.Should().NotBeNull();

            var taxResidenceValidationRule = policy.GetTaxResidenceValidationRule();
            taxResidenceValidationRule.Should().NotBeNull();
            taxResidenceValidationRule.AllowedCountries.Should().NotBeEmpty();

            var tinValidationRule = policy.GetTinValidationRule();
            tinValidationRule.Should().NotBeNull();
            tinValidationRule.AllowedTypes.Should().NotBeEmpty();
            tinValidationRule.AllowedTypes.Should().HaveCount(2);

            var selfieRules = policy.GetDocumentValidationRule(ActionType.Selfie);
            selfieRules.Should().NotBeNull();
            selfieRules.AllowedTypes.Should().ContainSingle().Which.Key.Should().Be(SelfieDocumentType.Video);

            var pofRules = policy.GetDocumentValidationRule(ActionType.ProofOfFunds);
            pofRules.Should().NotBeNull();
            pofRules.AllowedTypes.Should().ContainSingle().Which.Key.Should().Be(FundsDocumentType.Other);

            var poaRules = policy.GetDocumentValidationRule(ActionType.ProofOfAddress);
            poaRules.Should().NotBeNull();
            poaRules.AllowedTypes.Should().HaveCount(expectedPoaDocumentTypes.Length);
            poaRules.AllowedTypes.Select(dto => dto.Key).Should().BeEquivalentTo(expectedPoaDocumentTypes);
        }
        
        [Test]
        public async Task ShouldReadFallbackRules()
        {
            // Arrange
            var expectedRuleType = new[]
            {
                ActionType.Selfie, 
                ActionType.Tin, 
                ActionType.TaxResidence, 
                ActionType.W9Form, 
                ActionType.ProofOfAddress,
                ActionType.ProofOfFunds, 
                ActionType.ProofOfIdentity
            };
            var storage = Resolve<IValidationPolicyStorage>();
            var regions = Regions.Seed
                                 .Select(region => (region.Name, Country: region.Countries[Randomizer.Seed.Next(region.Countries.Length - 1)]))
                                 .ToArray();
            
            // Act
            var policies = await regions.Foreach(async region =>
            {
                var policy = await storage.GetAsync(ValidationPolicySelectionContext.Create(region.Country, region.Name));
                return (Region: region, Rules: policy);
            });

            // Assert
            using var _ = new AssertionScope();
            foreach (var policy in policies)
            {
                policy.Rules.Keys.Should()
                      .Contain(expectedRuleType,
                               $"Country: {policy.Region.Country}, Region: {policy.Region.Name}. Missed:{expectedRuleType.Except(policy.Rules.Keys).JoinIntoString()}");
                policy.Rules.Values.Should().NotContainNulls("Country: {policy.Region.Country}, Region: {policy.Region.Name}");
            }
        }
    }
}