using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries;
using WX.B2C.User.Verification.Integration.Tests.Builders;
using WX.B2C.User.Verification.Integration.Tests.Fixtures;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.DataAccess
{
    public class VerificationTaskRepositoryTests : BaseIntegrationTest
    {
        private readonly List<Guid> _users = new();

        private VerificationTaskFixture _verificationTaskFixture;

        protected override void RegisterModules(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<VerificationTaskFixture>().AsSelf().SingleInstance();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _verificationTaskFixture = Resolve<VerificationTaskFixture>();
            FsCheck.Arb.Register<CheckArbitrary>();
            FsCheck.Arb.Register<VerificationTaskArbitrary>();
            FsCheck.Arb.Register<TaskCheckArbitrary>();
            FsCheck.Arb.Register<TaskCollectionStepArbitrary>();
            FsCheck.Arb.Register<CollectionStepArbitrary>();
            FsCheck.Arb.Register<ArrayArbitrary<VerificationTaskSpecimen>>();
            FsCheck.Arb.Register<ArrayArbitrary<Guid>>();
        }

        [OneTimeTearDown]
        public Task OneTimeTearDown() => DbFixture.Clear(_users.ToArray());

        [Theory]
        public async Task ShouldSaveVerificationTask(VerificationTaskSpecimen specimen)
        {
            // Given
            _users.Add(specimen.UserId);
            var task = new VerificationTaskBuilder().From(specimen).Build();
            await _verificationTaskFixture.SaveCollectionSteps(task);
            var repository = Resolve<ITaskRepository>();

            // Act
            await repository.SaveAsync(task);

            // Assert
            var entity = DbFixture.DbContext.Find<Verification.DataAccess.Entities.VerificationTask>(task.Id);
            var entry = DbFixture.DbContext.Entry(entity);
            entry.Collection(x => x.CollectionSteps).Load();

            entity.Should().NotBeNull();
            entity.CollectionSteps.Should().HaveSameCount(specimen.CollectionSteps);
        }

        [Theory]
        public async Task ShouldFindVerificationTask(VerificationTaskSpecimen specimen)
        {
            // Given
            _users.Add(specimen.UserId);
            var task = new VerificationTaskBuilder().From(specimen).Build();
            await _verificationTaskFixture.SaveCollectionSteps(task);
            var repository = Resolve<ITaskRepository>();

            // Act
            await repository.SaveAsync(task);
            await _verificationTaskFixture.SaveChecks(task);

            // Assert
            var actual = await repository.GetAsync(task.Id);
            actual.Should().BeEquivalentTo(task);
        }

        [Theory]
        public async Task ShouldUpdateVerificationTask(VerificationTaskSpecimen oldSpecimen,
                                                       VerificationTaskSpecimen newSpecimen)
        {
            // Given
            _users.Add(oldSpecimen.UserId);
            var old = new VerificationTaskBuilder().From(oldSpecimen).Build();

            await _verificationTaskFixture.SaveCollectionSteps(old);
            var repository = Resolve<ITaskRepository>();
            await repository.SaveAsync(old);
            await _verificationTaskFixture.SaveChecks(old);

            // Arrange
            (newSpecimen.Id, newSpecimen.UserId) = (old.Id, old.UserId);
            var expected = new VerificationTaskBuilder().From(newSpecimen).Build();

            // Act
            await _verificationTaskFixture.SaveCollectionSteps(expected);
            await repository.SaveAsync(expected);
            await _verificationTaskFixture.SaveChecks(expected);

            // Assert
            var actual = await repository.GetAsync(expected.Id);
            actual.Should().BeEquivalentTo(expected, config => config.Excluding(t => t.Checks));
            actual.Checks.Should().BeEquivalentTo(old.Checks.Concat(expected.Checks));
        }

        /// <summary>
        /// Scenario: check is created and saved. In parallel task is read from db before check saved. Task is saved to db.
        /// When we read task again new check must be with performed check.
        /// Given Read task from DB
        /// And Save new check in DB
        /// When task is saved to DB
        /// Then check must exists
        /// </summary>
        [Theory]
        public async Task ShouldNotClearPerformedChecks(VerificationTaskSpecimen specimen, CheckSpecimen checkSpecimen)
        {
            // Given
            _users.Add(specimen.UserId);
            var task = new VerificationTaskBuilder().From(specimen).Build();
            await _verificationTaskFixture.SaveCollectionSteps(task);
            var repository = Resolve<ITaskRepository>();
            await repository.SaveAsync(task);

            task  = await repository.GetAsync(task.Id);

            var checkRepository = Resolve<ICheckRepository>();
            var check = new CheckBuilder().From(checkSpecimen).WithUserId(specimen.UserId).WithRelatedTasks(specimen.Id).Build();
            await checkRepository.SaveAsync(check);
            
            // Act
            await repository.SaveAsync(task);
            
            // Assert
            var entity = DbFixture.DbContext.Find<Verification.DataAccess.Entities.VerificationTask>(task.Id);
            var entry = DbFixture.DbContext.Entry(entity);
            await entry.Collection(x => x.PerformedChecks).LoadAsync();

            entry.Entity.PerformedChecks.Should().Contain(taskCheck => taskCheck.CheckId == check.Id);
        }
    }
}