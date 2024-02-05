using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries;
using WX.B2C.User.Verification.Integration.Tests.Builders;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.DataAccess
{
    class CollectionStepTests : BaseIntegrationTest
    {
        private readonly List<Guid> _users = new();

        private ICollectionStepRepository _sut;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _sut = Resolve<ICollectionStepRepository>();
            Arb.Register<CollectionStepArbitrary>();
        }

        [OneTimeTearDown]
        public Task OneTimeTearDown() => DbFixture.Clear(_users.ToArray());

        [Theory]
        public async Task SaveAsync_ShouldSaveCollectionStep(CollectionStepSpecimen specimen)
        {
            // Given
            _users.Add(specimen.UserId);
            var step = new CollectionStepBuilder().From(specimen).Build();

            // Act 
            await _sut.SaveAsync(step);

            // Assert
            var entity = DbFixture.DbContext.Find<Verification.DataAccess.Entities.CollectionStep>(step.Id);
            entity.Should().BeEquivalentTo(step, options => options.ExcludingMissingMembers());
        }

        [Theory]
        public async Task GetAsync_ShouldFindCollectionStep(CollectionStepSpecimen specimen)
        {
            // Given
            _users.Add(specimen.UserId);
            var step = new CollectionStepBuilder().From(specimen).Build();
            await _sut.SaveAsync(step);

            // Act
            var actual = await _sut.GetAsync(step.Id);

            // Assert
            actual.Should().BeEquivalentTo(step);
        }

        [Theory]
        public async Task FindRequestedAsync_ShouldFindCollectionStep(CollectionStepSpecimen specimen)
        {
            // Given
            _users.Add(specimen.UserId);
            specimen.State = CollectionStepState.Requested;
            var step = new CollectionStepBuilder().From(specimen).Build();
            await _sut.SaveAsync(step);

            // Act
            var actual = await _sut.FindNotCompletedAsync(step.UserId, step.XPath);

            // Assert
            actual.Should().BeEquivalentTo(step);
        }

        [Theory]
        public async Task FindRequestedAsync_ShouldNotFindCollectionStep_WhenCollectionStepCompleted(CollectionStepSpecimen specimen)
        {
            // Given
            _users.Add(specimen.UserId);
            specimen.State = CollectionStepState.Completed;
            var step = new CollectionStepBuilder().From(specimen).Build();
            await _sut.SaveAsync(step);

            // Act
            var actual = await _sut.FindNotCompletedAsync(step.UserId, step.XPath);

            // Assert
            actual.Should().BeNull();
        }

        [Theory]
        public async Task SaveAsync_ShouldUpdateCollectionStep(CollectionStepSpecimen oldSpecimen,
                                                               CollectionStepSpecimen newSpecimen)
        {
            // Given
            _users.Add(oldSpecimen.UserId);
            var old = new CollectionStepBuilder().From(oldSpecimen).Build();
            await _sut.SaveAsync(old);

            var saved = await _sut.GetAsync(old.Id);
            saved.Should().BeEquivalentTo(old);

            // Arrange
            (newSpecimen.Id, newSpecimen.UserId) = (old.Id, old.UserId);
            var expected = new CollectionStepBuilder().From(newSpecimen).Build();

            // Act
            await _sut.SaveAsync(expected);

            // Assert
            var actual = await _sut.GetAsync(expected.Id);
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
