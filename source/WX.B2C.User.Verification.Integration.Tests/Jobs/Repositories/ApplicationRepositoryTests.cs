using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries;
using WX.B2C.User.Verification.Integration.Tests.Builders;
using WX.B2C.User.Verification.Integration.Tests.Fixtures;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories;
using ITaskRepository = WX.B2C.User.Verification.Domain.ITaskRepository;

namespace WX.B2C.User.Verification.Integration.Tests.Jobs.Repositories
{
    [TestFixture]
    internal class ApplicationRepositoryTests : BaseIntegrationTest
    {
        private ApplicationRepository _repository;
        private VerificationTaskFixture _verificationTaskFixture;
        private ApplicationFixture _applicationFixture;

        protected override void RegisterModules(ContainerBuilder containerBuilder)
        {
            base.RegisterModules(containerBuilder);
            containerBuilder.RegisterDbQueryFactory();
            containerBuilder.RegisterType<VerificationTaskFixture>().AsSelf().SingleInstance();
            containerBuilder.RegisterType<ApplicationFixture>().AsSelf().SingleInstance();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _verificationTaskFixture = Resolve<VerificationTaskFixture>();
            _applicationFixture = Resolve<ApplicationFixture>();

            FsCheck.Arb.Register<ApplicationArbitrary>();
            FsCheck.Arb.Register<VerificationTaskArbitrary>();
            FsCheck.Arb.Register<TaskCollectionStepArbitrary>();
            FsCheck.Arb.Register<CollectionStepArbitrary>();
        }

        [SetUp]
        public void Setup()
        {
            _repository = new ApplicationRepository(Resolve<IQueryFactory>());
        }

        [Theory]
        public async Task ShouldLinkTaskToApplication(ApplicationSpecimen applicationSpecimen, VerificationTaskSpecimen[] tasks)
        {
            // Arrange
            var tasksIds = tasks.Select(specimen => specimen.Id).ToArray();
            await tasks.ForeachConsistently(async specimen =>
            {
                var task = new VerificationTaskBuilder().From(specimen).Build();
                await _verificationTaskFixture.SaveCollectionSteps(task);
                var taskRepository = Resolve<ITaskRepository>();
                await taskRepository.SaveAsync(task);
            });

            var repository = Resolve<Domain.IApplicationRepository>();
            await _applicationFixture.SaveRelationsAsync(applicationSpecimen);
            var application = new ApplicationBuilder().From(applicationSpecimen).Build();
            await repository.SaveAsync(application);

            // Act
            await _repository.LinkTasksAsync(applicationSpecimen.Id, tasksIds);

            // Assert
            var persistedTask = await repository.GetAsync(applicationSpecimen.Id);
            persistedTask.RequiredTasks.Select(task => task.Id).Should().Contain(tasksIds);
        }
    }
}