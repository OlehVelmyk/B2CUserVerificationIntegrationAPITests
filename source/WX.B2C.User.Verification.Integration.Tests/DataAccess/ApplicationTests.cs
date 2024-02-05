using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries;
using WX.B2C.User.Verification.Integration.Tests.Builders;
using WX.B2C.User.Verification.Integration.Tests.Fixtures;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.DataAccess
{
    internal class ApplicationTests : BaseIntegrationTest
    {
        private readonly List<Guid> _users = new();

        private IApplicationRepository _sut;
        private ApplicationFixture _applicationFixture;

        protected override void RegisterModules(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ApplicationFixture>().AsSelf().SingleInstance();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _sut = Resolve<IApplicationRepository>();
            _applicationFixture = Resolve<ApplicationFixture>();
            Arb.Register<ApplicationArbitrary>();
            Arb.Register<VerificationTaskArbitrary>();
        }

        [OneTimeTearDown]
        public Task OneTimeTearDown() => DbFixture.Clear(_users.ToArray());

        [Theory]
        public async Task SaveAsync_ShouldSaveApplication(ApplicationSpecimen specimen)
        {
            // Given
            _users.Add(specimen.UserId);
            await _applicationFixture.SaveRelationsAsync(specimen);
            var application = new ApplicationBuilder().From(specimen).Build();

            // Act 
            await _sut.SaveAsync(application);

            // Assert
            var entity = DbFixture.DbContext.Find<Verification.DataAccess.Entities.Application>(application.Id);
            entity.Should().BeEquivalentTo(application, options => options.ExcludingMissingMembers()
                                                                          .Excluding(x => x.RequiredTasks));
        }

        [Theory]
        public async Task GetAsync_ShouldFindApplication(ApplicationSpecimen specimen)
        {
            // Given
            _users.Add(specimen.UserId);
            await _applicationFixture.SaveRelationsAsync(specimen);
            var application = new ApplicationBuilder().From(specimen).Build();
            await _sut.SaveAsync(application);

            // Act
            var actual = await _sut.GetAsync(application.Id);

            // Assert
            actual.Should().BeEquivalentTo(application);
        }

        [Theory]
        public async Task SaveAsync_ShouldUpdateApplication(ApplicationSpecimen oldSpecimen,
                                                            ApplicationSpecimen newSpecimen)
        {
            // Given
            _users.Add(oldSpecimen.UserId);
            await _applicationFixture.SaveRelationsAsync(oldSpecimen);
            var old = new ApplicationBuilder().From(oldSpecimen).Build();
            await _sut.SaveAsync(old);

            var saved = await _sut.GetAsync(old.Id);
            saved.Should().BeEquivalentTo(old);

            // Arrange
            (newSpecimen.Id, newSpecimen.UserId) = (old.Id, old.UserId);
            await _applicationFixture.SaveRelationsAsync(newSpecimen);
            var expected = new ApplicationBuilder().From(newSpecimen).Build();

            // Act
            await _sut.SaveAsync(expected);

            // Assert
            var actual = await _sut.GetAsync(expected.Id);
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
