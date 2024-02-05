using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries;
using WX.B2C.User.Verification.Integration.Tests.Builders;
using WX.B2C.User.Verification.Integration.Tests.Fixtures;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories;
using ICollectionStepRepository = WX.B2C.User.Verification.Domain.DataCollection.ICollectionStepRepository;
using ITaskRepository = WX.B2C.User.Verification.Domain.ITaskRepository;

namespace WX.B2C.User.Verification.Integration.Tests.Jobs.Repositories
{
    [TestFixture]
    internal class TaskCollectionStepRepositoryTests : BaseIntegrationTest
    {
        private TaskCollectionStepRepository _repository;
        private VerificationTaskFixture _verificationTaskFixture;

        protected override void RegisterModules(ContainerBuilder containerBuilder)
        {
            base.RegisterModules(containerBuilder);
            containerBuilder.RegisterDbQueryFactory();
            containerBuilder.RegisterType<VerificationTaskFixture>().AsSelf().SingleInstance();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _verificationTaskFixture = Resolve<VerificationTaskFixture>();

            FsCheck.Arb.Register<VerificationTaskArbitrary>();
            FsCheck.Arb.Register<TaskCollectionStepArbitrary>();
            FsCheck.Arb.Register<CollectionStepArbitrary>();
        }

        [SetUp]
        public void Setup()
        {
            _repository = new TaskCollectionStepRepository(Resolve<IQueryFactory>());
        }

        [Theory]
        public async Task ShouldAddCollectionStepToTask(VerificationTaskSpecimen specimen, CollectionStepSpecimen step)
        {
            // Arrange
            var task = new VerificationTaskBuilder().From(specimen).Build();
            await _verificationTaskFixture.SaveCollectionSteps(task);
            var taskRepository = Resolve<ITaskRepository>();
            await taskRepository.SaveAsync(task);

            var stepRepository = Resolve<ICollectionStepRepository>();
            await stepRepository.SaveAsync(new CollectionStepBuilder().From(step).Build());

            // Act
            await _repository.AddNewCollectionStepsAsync(task.Id, new[] { step.Id });

            // Assert
            var persistedTask = await taskRepository.GetAsync(task.Id);
            persistedTask.CollectionSteps.Select(step => step.Id).Should().Contain(step.Id);
        }

        [Theory]
        public async Task ShouldNotAddCollectionStepToTask(VerificationTaskSpecimen specimen, TaskCollectionStep existingStep)
        {
            // Arrange
            specimen.CollectionSteps.Add(existingStep);
            var task = new VerificationTaskBuilder().From(specimen).Build();
            await _verificationTaskFixture.SaveCollectionSteps(task);
            var taskRepository = Resolve<ITaskRepository>();
            await taskRepository.SaveAsync(task);

            // Act
            await _repository.AddNewCollectionStepsAsync(task.Id, new[] { existingStep.Id });

            // Assert
            var persistedTask = await taskRepository.GetAsync(task.Id);
            persistedTask.CollectionSteps.Select(step => step.Id).Should().Contain(existingStep.Id);
        }

        [Theory]
        public async Task ShouldAddOnlyAbsentCollectionStepToTask(VerificationTaskSpecimen specimen,
                                                                  TaskCollectionStep existingStep,
                                                                  CollectionStepSpecimen newStep)
        {
            // Arrange
            specimen.CollectionSteps.Add(existingStep);
            var task = new VerificationTaskBuilder().From(specimen).Build();
            await _verificationTaskFixture.SaveCollectionSteps(task);
            var taskRepository = Resolve<ITaskRepository>();
            await taskRepository.SaveAsync(task);

            var stepRepository = Resolve<ICollectionStepRepository>();
            await stepRepository.SaveAsync(new CollectionStepBuilder().From(newStep).Build());

            // Act
            await _repository.AddNewCollectionStepsAsync(task.Id, new[] { existingStep.Id, newStep.Id });

            // Assert
            var persistedTask = await taskRepository.GetAsync(task.Id);
            persistedTask.CollectionSteps.Select(step => step.Id).Should().Contain(existingStep.Id);
            persistedTask.CollectionSteps.Select(step => step.Id).Should().Contain(newStep.Id);
        }
    }
}