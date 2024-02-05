using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using FluentAssertions.Equivalency;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries;
using WX.B2C.User.Verification.Integration.Tests.Builders;
using WX.B2C.User.Verification.Integration.Tests.Fixtures;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.DataAccess
{
    internal class VerificationTaskStorageTests : BaseIntegrationTest
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
        }

        [OneTimeTearDown]
        public Task OneTimeTearDown() => DbFixture.Clear(_users.ToArray());

        [Theory]
        public async Task ShouldFindVerificationTask(VerificationTaskSpecimen specimen)
        {
            // Given
            _users.Add(specimen.UserId);
            var task = new VerificationTaskBuilder().From(specimen).Build();
            await _verificationTaskFixture.SaveCollectionSteps(task);
            var repository = Resolve<ITaskRepository>();
            var storage = Resolve<ITaskStorage>();
            await repository.SaveAsync(task);
            await _verificationTaskFixture.SaveChecks(task);

            // Act
            var actual = await storage.GetAsync(task.Id);

            // Assert
            actual.Should().BeEquivalentTo(specimen, _assertionSettings);
            actual.Checks.Should().BeEquivalentTo(specimen.PerformedChecks);
        }

        [Theory]
        public async Task ShouldFindVerificationTasks(VerificationTaskSpecimen[] specimens, Guid userId)
        {
            // Given
            var repository = Resolve<ITaskRepository>();
            var storage = Resolve<ITaskStorage>();
            _users.Add(userId);

            foreach (var specimen in specimens)
            {
                specimen.UserId = userId;
                var task = new VerificationTaskBuilder().From(specimen).Build();
                await _verificationTaskFixture.SaveCollectionSteps(task);
                await repository.SaveAsync(task);
                await _verificationTaskFixture.SaveChecks(task);
            }

            // Act
            var result = await storage.GetAllAsync(userId);

            // Assert
            result.Should().NotBeNullOrEmpty().And.HaveSameCount(specimens);
            result.Foreach(actual =>
            {
                actual.Should().NotBeNull();
                var specimen = specimens.First(specimen => specimen.Id == actual.Id);
                actual.Should().BeEquivalentTo(specimen, _assertionSettings);
            });
        }

        [Theory]
        public async Task ShouldFindVerificationTask_WhenManyOthersExist(VerificationTaskSpecimen[] specimens, VerificationTaskSpecimen specimen, Guid userId)
        {
            // Given
            var repository = Resolve<ITaskRepository>();
            var storage = Resolve<ITaskStorage>();
            _users.Add(userId);

            specimens = specimens.Where(s => s.Type != specimen.Type).Prepend(specimen).ToArray();
            foreach (var s in specimens)
            {
                s.UserId = userId;
                var task = new VerificationTaskBuilder().From(s).Build();
                await _verificationTaskFixture.SaveCollectionSteps(task);
                await repository.SaveAsync(task);
                await _verificationTaskFixture.SaveChecks(task);
            }

            // Act
            var result = await storage.GetAllAsync(userId, specimen.Type);

            // Assert
            var actual = result.Should().HaveCount(1).And.Subject.First();
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(specimen, _assertionSettings);
        }

        [Theory]
        public async Task ShouldFindVerificationTasks(VerificationTaskSpecimen[] specimens, CheckSpecimen checkSpecimen, Guid userId)
        {
            // Given
            var repository = Resolve<ITaskRepository>();
            var checkRepository = Resolve<ICheckRepository>();
            var storage = Resolve<ITaskStorage>();
            _users.Add(userId);

            foreach (var specimen in specimens)
            {
                specimen.UserId = userId;
                var task = new VerificationTaskBuilder().From(specimen).Build();
                await _verificationTaskFixture.SaveCollectionSteps(task);

                task = new VerificationTaskBuilder().From(specimen).Build();
                await repository.SaveAsync(task);
                await _verificationTaskFixture.SaveChecks(task);
            }

            var check = new CheckBuilder().From(checkSpecimen)
                                          .WithUserId(userId)
                                          .WithRelatedTasks(specimens.Select(s => s.Id).ToArray())
                                          .Build();
            await checkRepository.SaveAsync(check);

            // Act
            var result = await storage.FindByCheckIdAsync(check.Id);

            // Assert
            result.Should().NotBeNullOrEmpty().And.HaveSameCount(specimens);
            result.Foreach(actual =>
            {
                actual.Should().NotBeNull();
                var specimen = specimens.First(specimen => specimen.Id == actual.Id);
                actual.Should().BeEquivalentTo(specimen, _assertionSettings);
            });
        }

        [Theory]
        public async Task ShouldFindTasksByCollectionStep(VerificationTaskSpecimen[] specimens, TaskCollectionStep taskStep, Guid userId)
        {
            var repository = Resolve<ITaskRepository>();
            var storage = Resolve<ITaskStorage>();
            _users.Add(userId);

            await _verificationTaskFixture.SaveCollectionStepAsync(taskStep, userId);
            foreach (var specimen in specimens)
            {
                specimen.UserId = userId;
                var task = new VerificationTaskBuilder().From(specimen).Build();
                await _verificationTaskFixture.SaveCollectionSteps(task);

                specimen.CollectionSteps.Add(taskStep);
                task = new VerificationTaskBuilder().From(specimen).Build();
                await repository.SaveAsync(task);
                await _verificationTaskFixture.SaveChecks(task);
            };

            var result = await storage.FindByStepIdAsync(taskStep.Id);

            result.Should().NotBeNullOrEmpty().And.HaveSameCount(specimens);
            result.Foreach(actual =>
            {
                actual.Should().NotBeNull();
                var specimen = specimens.First(specimen => specimen.Id == actual.Id);
                actual.Should().BeEquivalentTo(specimen, _assertionSettings);
            });
        }


        [Theory]
        public async Task ShouldFindVerificationTasks(VerificationTaskSpecimen[] specimens, Guid[] variantIds, Guid userId)
        {
            // Given
            var repository = Resolve<ITaskRepository>();
            var storage = Resolve<ITaskStorage>();
            _users.Add(userId);

            var count = 0;
            foreach (var specimen in specimens)
            {
                specimen.UserId = userId;
                specimen.VariantId = variantIds[count++ % variantIds.Length];
                var task = new VerificationTaskBuilder().From(specimen).Build();
                await _verificationTaskFixture.SaveCollectionSteps(task);
                await repository.SaveAsync(task);
                await _verificationTaskFixture.SaveChecks(task);
            };

            // Act
            var result = await storage.FindAsync(userId, variantIds);

            // Assert
            result.Should().NotBeNullOrEmpty().And.HaveSameCount(specimens);
            result.Foreach(actual =>
            {
                actual.Should().NotBeNull();
                var specimen = specimens.First(specimen => specimen.Id == actual.Id);
                var expected = new VerificationTaskBuilder().From(specimen).Build();
                actual.Should().BeEquivalentTo(expected);
            });
        }

        private readonly Func<EquivalencyAssertionOptions<VerificationTaskSpecimen>, EquivalencyAssertionOptions<VerificationTaskSpecimen>>
            _assertionSettings = options => options.ExcludingMissingMembers();
    }
}
