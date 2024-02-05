using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Integration.Tests.DataAccess
{
    [TestFixture]
    public class VerificationPolicyStorageTests : BaseIntegrationTest
    {
        private const string DefinedState = nameof(DefinedState);
        private const string Undefined = nameof(Undefined);
        private const string Global = nameof(Global);

        private IVerificationPolicyStorage _storage;
        private List<VerificationPolicy> _testPolicies;

        [OneTimeSetUp]
        public void Setup()
        {
            _storage = Resolve<IVerificationPolicyStorage>();
            _testPolicies = new List<VerificationPolicy>
            {
                new VerificationPolicy
                {
                    RegionType = RegionType.State,
                    Region = DefinedState,
                    Id = Guid.NewGuid(),
                    Name = DefinedState
                }
            };

            DbFixture.DbContext.AddRange(_testPolicies);
            var entities = DbFixture.DbContext.SaveChanges();
            Assert.AreEqual(_testPolicies.Count, entities);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            foreach (var entity in _testPolicies.Select(verificationPolicy =>
                                                            DbFixture.DbContext.Find<VerificationPolicy>(verificationPolicy.Id)))
            {
                DbFixture.DbContext.Remove((object) entity);
            }
            var entities = DbFixture.DbContext.SaveChanges();
            Assert.AreEqual(_testPolicies.Count, entities);
        }

        [Test]
        public async Task ShouldReadPolicyForUK()
        {
            // Arrange   
            var country = "GB";
            var region = "EU";

            // Act
            Func<Task<VerificationPolicyDto>> act = () => _storage.GetAsync(VerificationPolicySelectionContext.Create(country, region));

            // Assert
            var result = await act.Should().NotThrowAsync();
            result.Subject.Should().NotBeNull();
        }

        [TestCase(DefinedState, "US", "US", DefinedState)]
        [TestCase(Undefined, "US", "US", "US")]
        [TestCase(Undefined, "FR", "EEA", "EEA")]
        [TestCase(Undefined, "CA", "CA", Global)]
        public async Task ShouldReadPolicyByPriority(string state,
                                                     string country,
                                                     string region,
                                                     string expectedPolicyName)
        {
            // Arrange
            var expected = await DbFixture.DbContext.Set<VerificationPolicy>()
                                          .Where(policy => policy.Region == expectedPolicyName)
                                          .SingleOrDefaultAsync();

            // Act
            Func<Task<VerificationPolicyDto>> act = () =>
                _storage.GetAsync(VerificationPolicySelectionContext.Create(country, region, state));

            // Assert
            var result = await act.Should().NotThrowAsync();
            result.Subject.Should().NotBeNull();
            result.Subject.Id.Should().Be(expected.Id);
        }

        [TestCase(DefinedState, "US", "US", DefinedState)]
        [TestCase(Undefined, "US", "US", "US")]
        [TestCase(Undefined, "FR", "EEA", "EEA")]
        [TestCase(Undefined, "CA", "CA", Global)]
        public async Task ShouldReadPolicyIdByPriority(string state,
                                                       string country,
                                                       string region,
                                                       string expectedPolicyName)
        {
            // Arrange
            var expected = await DbFixture.DbContext.Set<VerificationPolicy>()
                                          .Where(policy => policy.Region == expectedPolicyName)
                                          .SingleOrDefaultAsync();

            // Act
            Func<Task<Guid>> act = () => _storage.GetIdAsync(VerificationPolicySelectionContext.Create(country, region, state));

            // Assert
            var result = await act.Should().NotThrowAsync();
            result.Subject.Should().Be(expected.Id);
        }
    }
}