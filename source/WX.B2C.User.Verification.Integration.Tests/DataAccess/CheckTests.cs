using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Equivalency;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries;
using WX.B2C.User.Verification.Integration.Tests.Builders;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.DataAccess
{
    internal class CheckTests : BaseIntegrationTest
    {
        private readonly List<Guid> _users = new();

        private ICheckRepository _sut;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _sut = Resolve<ICheckRepository>();
            Arb.Register<CheckArbitrary>();
        }

        [OneTimeTearDown]
        public Task OneTimeTearDown() => DbFixture.Clear(_users.ToArray());

        [Theory]
        public async Task SaveAsync_ShouldSaveCheck(CheckSpecimen specimen)
        {
            // Given
            _users.Add(specimen.UserId);
            var check = new CheckBuilder().From(specimen).Build();

            // Act 
            await _sut.SaveAsync(check);

            // Assert
            var entity = DbFixture.DbContext.Find<Verification.DataAccess.Entities.Check>(check.Id);
            entity.Should().NotBeNull();
        }

        [Theory]
        public async Task GetAsync_ShouldFindCheck(CheckSpecimen specimen)
        {
            // Given
            _users.Add(specimen.UserId);
            var check = new CheckBuilder().From(specimen).Build();
            await _sut.SaveAsync(check);

            // Act
            var actual = await _sut.GetAsync(check.Id);

            // Assert
            actual.Should().BeEquivalentTo(check, _assertionSettings); // TODO: Fix reading of RelatedTasks
        }

        [Theory]
        public async Task SaveAsync_ShouldUpdateCheck(CheckSpecimen oldSpecimen, CheckSpecimen newSpecimen)
        {
            // Given
            _users.Add(oldSpecimen.UserId);
            var old = new CheckBuilder().From(oldSpecimen).Build();
            await _sut.SaveAsync(old);

            // Arrange
            (newSpecimen.Id, newSpecimen.UserId) = (old.Id, old.UserId);
            var expected = new CheckBuilder().From(newSpecimen).Build();

            // Act
            await _sut.SaveAsync(expected);

            // Assert
            var actual = await _sut.GetAsync(expected.Id);
            actual.Should().BeEquivalentTo(expected, _assertionSettings);
        }

        private readonly Func<EquivalencyAssertionOptions<Domain.Models.Check>, EquivalencyAssertionOptions<Domain.Models.Check>>
            _assertionSettings = options =>
            {
                return options.Excluding(c => c.RelatedTasks)
                              .Using<CheckExecutionContext>(ctx =>
                              {
                                  ctx.Subject?.InputData.Should().BeEquivalentTo(ctx.Expectation?.InputData);
                                  ctx.Subject?.ExternalData.Should().BeEquivalentTo(ctx.Expectation?.ExternalData);
                              }).WhenTypeIs<CheckExecutionContext>();
            };
    }
}
