using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries;
using WX.B2C.User.Verification.Integration.Tests.Fixtures;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories;
using ITaskRepository = WX.B2C.User.Verification.Domain.ITaskRepository;

namespace WX.B2C.User.Verification.Integration.Tests.Jobs.Repositories
{
    [TestFixture]
    internal class TaskRepositoryTests : BaseIntegrationTest
    {
        private TaskRepository _repository;

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
            Resolve<VerificationTaskFixture>();
            Resolve<ApplicationFixture>();

            FsCheck.Arb.Register<ApplicationArbitrary>();
            FsCheck.Arb.Register<VerificationTaskArbitrary>();
            FsCheck.Arb.Register<TaskCollectionStepArbitrary>();
            FsCheck.Arb.Register<CollectionStepArbitrary>();
        }

        [SetUp]
        public void Setup()
        {
            _repository = new TaskRepository(Resolve<IQueryFactory>());
        }

        [Theory]
        public async Task ShouldCreateTask(VerificationTaskSpecimen taskSpecimen)
        {
            // Arrange
            var template = new TaskTemplate
            {
                UserId = taskSpecimen.UserId,
                Type = taskSpecimen.Type,
                VariantId = taskSpecimen.VariantId
            };

            // Act
            var id = await _repository.CreateAsync(template);

            // Assert
            var persistedTask = await Resolve<ITaskRepository>().GetAsync(id);
            persistedTask.UserId.Should().Be(taskSpecimen.UserId);
            persistedTask.Type.Should().Be(taskSpecimen.Type);
            persistedTask.VariantId.Should().Be(taskSpecimen.VariantId);
        }
    }
}