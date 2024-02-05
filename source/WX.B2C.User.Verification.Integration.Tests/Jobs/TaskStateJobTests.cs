using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using FsCheck;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Quartz;
using Serilog;
using WX.B2C.User.Verification.BlobStorage.Storages;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Services.IoC;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Shared;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries;
using WX.B2C.User.Verification.Integration.Tests.Builders;
using WX.B2C.User.Verification.Integration.Tests.Fixtures;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories;
using WX.B2C.User.Verification.Worker.Jobs.Jobs;
using WX.B2C.User.Verification.Worker.Jobs.Providers;

namespace WX.B2C.User.Verification.Integration.Tests.Jobs
{
    using static Worker.Jobs.Models.Constants;

    public class TaskStateJobTests : BaseIntegrationTest
    {
        private TaskStateJob _sut;
        private JobDataMap _dataMap;
        private IJobExecutionContext _context;
        private ICsvBlobStorage _csvBlobStorage;
        private Domain.ITaskRepository _taskRepository;

        protected override void RegisterModules(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterDbQueryFactory()
                            .RegisterCoreServices()
                            .RegisterSystemClock();

            containerBuilder.Register(c => Substitute.For<IEventPublisher>())
                            .SingleInstance();
            containerBuilder.RegisterType<ApplicationFixture>()
                            .SingleInstance();
            containerBuilder.RegisterType<TaskRepository>()
                            .As<ITaskRepository>()
                            .SingleInstance();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _context = Substitute.For<IJobExecutionContext>();
            _dataMap = Substitute.For<JobDataMap>();
            _csvBlobStorage = Substitute.For<ICsvBlobStorage>();
            _context.MergedJobDataMap.Returns(_ => _dataMap);
            _taskRepository = Resolve<Domain.ITaskRepository>();

            var dataProvider = new TaskDataProvider(Resolve<IQueryFactory>(), _csvBlobStorage);
            _sut = new TaskStateJob(Resolve<ITaskService>(),
                                    Resolve<ITaskRepository>(),
                                    dataProvider,
                                    Resolve<ILogger>());

            Arb.Register<VerificationTaskArbitrary>();
            Arb.Register<VerificationTaskNoRelationsArbitrary>();
            Arb.Register<ArrayArbitrary<VerificationTaskNoRelationsSpecimen>>();
        }

        [Theory(MaxTest = 5)]
        public async Task ShouldCompeteTasks(VerificationTaskNoRelationsSpecimen[] testData, TaskResult result, bool useActors)
        {
            // Given
            foreach (var specimen in testData)
            {
                var task = new VerificationTaskBuilder().From(specimen).Build();
                await _taskRepository.SaveAsync(task);
            }

            // Arrange
            var jobSettings = new TaskStateJobSettingsBuilder().WithTaskState(TaskState.Completed)
                                                               .WithTaskResult(result)
                                                               .UseActors(useActors)
                                                               .Build();
            
            _dataMap.GetString(JobSettings).Returns(JsonConvert.SerializeObject(jobSettings));

            var csvData = testData.Select(specimen => new TaskDataProvider.CsvData { UserId = specimen.UserId, TaskType = specimen.Type });
            _csvBlobStorage.GetAsync<TaskDataProvider.CsvData>(Arg.Is("jobs"), jobSettings.FileName).Returns(csvData.ToArray());

            // Act
            await _sut.Execute(_context);

            // Assert
            foreach (var task in testData)
            {
                var persistedTask = await _taskRepository.GetAsync(task.Id);
                persistedTask.State.Should().Be(TaskState.Completed);
                persistedTask.Result.Should().Be(result);
            }
        }


        [Theory]
        public async Task ShouldIncompleteTasks(VerificationTaskNoRelationsSpecimen[] testData, bool useActors)
        {
            // Given
            foreach (var specimen in testData)
            {
                var task = new VerificationTaskBuilder().From(specimen).Build();
                await _taskRepository.SaveAsync(task);
            }

            // Arrange
            var jobSettings = new TaskStateJobSettingsBuilder().WithTaskState(TaskState.Incomplete).UseActors(useActors).Build();
            _dataMap.GetString(JobSettings).Returns(JsonConvert.SerializeObject(jobSettings));

            var csvData = testData.Select(specimen => new TaskDataProvider.CsvData { UserId = specimen.UserId, TaskType = specimen.Type });
            _csvBlobStorage.GetAsync<TaskDataProvider.CsvData>(Arg.Is("jobs"), jobSettings.FileName).Returns(csvData.ToArray());

            // Act
            await _sut.Execute(_context);

            // Assert
            foreach (var task in testData)
            {
                var persistedTask = await _taskRepository.GetAsync(task.Id);
                persistedTask.State.Should().Be(TaskState.Incomplete);
            }
        }

        [Theory]
        public async Task ShouldBeIdempotent(VerificationTaskNoRelationsSpecimen[] testData, TaskState taskState, bool useActors)
        {
            // Given
            foreach (var specimen in testData)
            {
                var task = new VerificationTaskBuilder().From(specimen).Build();
                await _taskRepository.SaveAsync(task);
            }

            // Arrange
            var jobSettings = new TaskStateJobSettingsBuilder().WithTaskState(taskState).UseActors(useActors).Build();
            _dataMap.GetString(JobSettings).Returns(JsonConvert.SerializeObject(jobSettings));

            var csvData = testData.Select(specimen => new TaskDataProvider.CsvData { UserId = specimen.UserId, TaskType = specimen.Type });
            _csvBlobStorage.GetAsync<TaskDataProvider.CsvData>(Arg.Is("jobs"), jobSettings.FileName).Returns(csvData.ToArray());

            // Act
            await _sut.Execute(_context);
            await _sut.Execute(_context);

            // Assert
            foreach (var task in testData)
            {
                var persistedTask = await _taskRepository.GetAsync(task.Id);
                persistedTask.State.Should().Be(taskState);
            }
        }
    }
}