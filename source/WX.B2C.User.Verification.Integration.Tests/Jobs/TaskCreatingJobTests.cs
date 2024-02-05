using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using FsCheck;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Quartz;
using Serilog.Core;
using WX.B2C.User.Verification.BlobStorage.Storages;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Shared;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries;
using WX.B2C.User.Verification.Integration.Tests.Builders;
using WX.B2C.User.Verification.Integration.Tests.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Fixtures;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.IoC;
using WX.B2C.User.Verification.Worker.Jobs.Jobs;
using WX.B2C.User.Verification.Worker.Jobs.Providers.Tasks;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;
using static WX.B2C.User.Verification.Worker.Jobs.Providers.Tasks.TaskCreatingDataProvider;
using IApplicationJobRepository = WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories.IApplicationRepository;
using ITasJobkRepository = WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories.ITaskRepository;

namespace WX.B2C.User.Verification.Integration.Tests.Jobs
{
    internal class TaskCreatingJobTests : BaseIntegrationTest
    {
        private TaskCreatingJob _sut;
        private JobDataMap _dataMap;
        private IJobExecutionContext _context;
        private ICsvBlobStorage _csvBlobStorage;
        private TaskCreatingJobSettings _jobSettings;

        protected override void RegisterModules(ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(c => Substitute.For<IEventPublisher>()).SingleInstance();
            containerBuilder.RegisterType<ApplicationFixture>().SingleInstance();
            containerBuilder.RegisterDbQueryFactory()
                            .RegisterJobRepositories()
                            .RegisterJobServices()
                            .RegisterJobDataAggregationServices();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _jobSettings = new TaskCreatingJobSettings
            {
                ProcessBatchSize = 1,
                ReadingBatchSize = 1,
                DelayInMillisecondsAfterBatch = 0,
                MaxErrorCount = 0,
                FileName = string.Empty,
            };

            _context = Substitute.For<IJobExecutionContext>();
            _dataMap = Substitute.For<JobDataMap>();
            _csvBlobStorage = Substitute.For<ICsvBlobStorage>();
            _context.MergedJobDataMap.Returns(_ => _dataMap);

            var dataProvider = new TaskCreatingDataProvider(Resolve<IQueryFactory>(),
                                                            _csvBlobStorage,
                                                            Resolve<ITaskCreatingDataAggregationService>());
            _sut = new TaskCreatingJob(Resolve<IApplicationJobRepository>(),
                                       Resolve<ITasJobkRepository>(),
                                       dataProvider,
                                       Logger.None);

            Arb.Register<ApplicationArbitrary>();
            Arb.Register<VerificationTaskArbitrary>();
            Arb.Register<ApplicationNoRelationsArbitrary>();
            Arb.Register<VerificationTaskNoRelationsArbitrary>();
            Arb.Register<PolicyTaskArbitrary>();
            Arb.Register<UsPolicyTaskArbitrary>();
            Arb.Register<GbPolicyTaskArbitrary>();
            Arb.Register<GbPoFPolicyTaskArbitrary>();
            Arb.Register<IdentityPolicyTaskArbitrary>();
            Arb.Register<UsPolicyTasksArbitrary>();
            Arb.Register<ArrayArbitrary<ApplicationNoRelationsSpecimen>>();
            Arb.Register<ArrayArbitrary<VerificationTaskNoRelationsSpecimen>>();
        }

        [Theory]
        public async Task ShouldCreateTask(ApplicationNoRelationsSpecimen appSpecimen, PolicyTaskSpecimen policySpecimen)
        {
            // Arrange
            var taskType = policySpecimen.TaskType;
            appSpecimen.PolicyId = policySpecimen.PolicyId;

            var application = new ApplicationBuilder().From(appSpecimen).Build();
            var applicationRepository = Resolve<IApplicationRepository>();
            await applicationRepository.SaveAsync(application);

            _jobSettings.TaskTypes = new[] { taskType };
            _dataMap.GetString(Worker.Jobs.Models.Constants.JobSettings).Returns(JsonConvert.SerializeObject(_jobSettings));

            var csvData = new CsvData { UserId = appSpecimen.UserId };
            _csvBlobStorage.GetAsync<CsvData>(Arg.Is("jobs"), Arg.Any<string>()).Returns(new[] { csvData });

            // Act
            await _sut.Execute(_context);

            // Assert
            var persistedApplication = await applicationRepository.GetAsync(application.Id);
            var applicationTasks = persistedApplication.RequiredTasks.Where(t => t.TaskType == taskType);
            var applicationTask = applicationTasks.Should().HaveCount(1).And.Subject.First();

            var tasks = await Resolve<ITaskStorage>().GetAllAsync(application.UserId, taskType);
            var task = tasks.Should().HaveCount(1).And.Subject.First();
            task.Id.Should().Be(applicationTask.Id);
            task.State.Should().Be(TaskState.Incomplete);
            task.VariantId.Should().Be(policySpecimen.TaskVariantId);
        }
        
        [Theory]
        public async Task ShouldCreateTask_WhenTaskFromTemplates(ApplicationNoRelationsSpecimen appSpecimen, GbPoFPolicyTaskSpecimen policySpecimen)
        {
            // Arrange
            var taskType = policySpecimen.TaskType;
            appSpecimen.PolicyId = policySpecimen.PolicyId;

            var application = new ApplicationBuilder().From(appSpecimen).Build();
            var applicationRepository = Resolve<IApplicationRepository>();
            await applicationRepository.SaveAsync(application);

            _jobSettings.TaskTypes = new[] { taskType };
            _dataMap.GetString(Worker.Jobs.Models.Constants.JobSettings).Returns(JsonConvert.SerializeObject(_jobSettings));

            var csvData = new CsvData { UserId = appSpecimen.UserId };
            _csvBlobStorage.GetAsync<CsvData>(Arg.Is("jobs"), Arg.Any<string>()).Returns(new[] { csvData });

            // Act
            await _sut.Execute(_context);

            // Assert
            var persistedApplication = await applicationRepository.GetAsync(application.Id);
            var applicationTasks = persistedApplication.RequiredTasks.Where(t => t.TaskType == taskType);
            var applicationTask = applicationTasks.Should().HaveCount(1).And.Subject.First();

            var tasks = await Resolve<ITaskStorage>().GetAllAsync(application.UserId, taskType);
            var task = tasks.Should().HaveCount(1).And.Subject.First();
            task.Id.Should().Be(applicationTask.Id);
            task.State.Should().Be(TaskState.Incomplete);
            task.VariantId.Should().Be(policySpecimen.TaskVariantId);
        }

        [Theory]
        public async Task ShouldCreateTask_WhenUserHasTasksOfOtherTypes(ApplicationNoRelationsSpecimen appSpecimen,
                                                                        VerificationTaskNoRelationsSpecimen[] existingTasks,
                                                                        PolicyTaskSpecimen policySpecimen)
        {
            // Arrange
            var taskType = policySpecimen.TaskType;
            appSpecimen.PolicyId = policySpecimen.PolicyId;
            appSpecimen.RequiredTasks = existingTasks.Where(task => task.Type != taskType).ToArray();

            await Resolve<ApplicationFixture>().SaveRelationsAsync(appSpecimen);
            var application = new ApplicationBuilder().From(appSpecimen).Build();
            var applicationRepository = Resolve<IApplicationRepository>();
            await applicationRepository.SaveAsync(application);

            _jobSettings.TaskTypes = new[] { taskType };
            _dataMap.GetString(Worker.Jobs.Models.Constants.JobSettings).Returns(JsonConvert.SerializeObject(_jobSettings));

            var csvData = new CsvData { UserId = appSpecimen.UserId };
            _csvBlobStorage.GetAsync<CsvData>(Arg.Is("jobs"), Arg.Any<string>()).Returns(new[] { csvData });

            // Act
            await _sut.Execute(_context);

            // Assert
            var persistedApplication = await applicationRepository.GetAsync(application.Id);
            var applicationTasks = persistedApplication.RequiredTasks.Where(t => t.TaskType == taskType);
            var applicationTask = applicationTasks.Should().HaveCount(1).And.Subject.First();

            var tasks = await Resolve<ITaskStorage>().GetAllAsync(application.UserId, taskType);
            var task = tasks.Should().HaveCount(1).And.Subject.First();
            task.Id.Should().Be(applicationTask.Id);
            task.State.Should().Be(TaskState.Incomplete);
            task.VariantId.Should().Be(policySpecimen.TaskVariantId);
        }

        [Theory]
        public async Task ShouldCreateTasks(ApplicationNoRelationsSpecimen appSpecimen, PolicyTasksSpecimen<UsPolicyTaskSpecimen> policyTasksSpecimen)
        {
            // Arrange
            appSpecimen.PolicyId = policyTasksSpecimen.Select(pt => pt.PolicyId).First();

            var application = new ApplicationBuilder().From(appSpecimen).Build();
            var applicationRepository = Resolve<IApplicationRepository>();
            await applicationRepository.SaveAsync(application);

            _jobSettings.TaskTypes = policyTasksSpecimen.Select(pt => pt.TaskType).ToArray();
            _dataMap.GetString(Worker.Jobs.Models.Constants.JobSettings).Returns(JsonConvert.SerializeObject(_jobSettings));

            var csvData = new CsvData { UserId = appSpecimen.UserId };
            _csvBlobStorage.GetAsync<CsvData>(Arg.Is("jobs"), Arg.Any<string>()).Returns(new[] { csvData });

            // Act
            await _sut.Execute(_context);

            // Assert
            var persistedApplication = await applicationRepository.GetAsync(application.Id);
            var applicationTasks = persistedApplication.RequiredTasks;

            var tasks = await Resolve<ITaskStorage>().GetAllAsync(application.UserId);
            tasks.Select(t => t.Id).Should().BeEquivalentTo(applicationTasks.Select(at => at.Id));
            tasks.Should().HaveSameCount(policyTasksSpecimen);
            tasks.Select(t => (t.VariantId, t.Type)).Should().BeEquivalentTo(policyTasksSpecimen.Select(pt => (pt.TaskVariantId, pt.TaskType)));
        }

        [Theory]
        public async Task ShouldNotCreateTask_WhenUserAlreadyHasTask(ApplicationNoRelationsSpecimen appSpecimen,
                                                                     VerificationTaskNoRelationsSpecimen existingTask, 
                                                                     PolicyTaskSpecimen policySpecimen)
        {
            // Arrange
            var taskType = policySpecimen.TaskType;
            appSpecimen.PolicyId = policySpecimen.PolicyId;
            existingTask.VariantId = policySpecimen.TaskVariantId;
            existingTask.Type = policySpecimen.TaskType;
            appSpecimen.RequiredTasks = new[] { existingTask };

            await Resolve<ApplicationFixture>().SaveRelationsAsync(appSpecimen);
            var application = new ApplicationBuilder().From(appSpecimen).Build();
            var applicationRepository = Resolve<IApplicationRepository>();
            await applicationRepository.SaveAsync(application);

            _jobSettings.TaskTypes = new[] { taskType };
            _dataMap.GetString(Worker.Jobs.Models.Constants.JobSettings).Returns(JsonConvert.SerializeObject(_jobSettings));

            var csvData = new CsvData { UserId = appSpecimen.UserId };
            _csvBlobStorage.GetAsync<CsvData>(Arg.Is("jobs"), Arg.Any<string>()).Returns(new[] { csvData });

            // Act
            await _sut.Execute(_context);

            // Assert
            var persistedApplication = await applicationRepository.GetAsync(application.Id);
            var applicationTasks = persistedApplication.RequiredTasks.Where(t => t.TaskType == taskType);
            var applicationTask = applicationTasks.Should().HaveCount(1).And.Subject.First();

            var tasks = await Resolve<ITaskStorage>().GetAllAsync(application.UserId, taskType);
            var task = tasks.Should().HaveCount(1).And.Subject.First();
            task.Id.Should().Be(applicationTask.Id).And.Subject.Should().Be(existingTask.Id);
        }

        [Theory]
        public async Task ShouldNotCreateTask_WhenApplicationNotExist(Guid userId, TaskType taskType)
        {
            // Arrange
            _jobSettings.TaskTypes = new[] { taskType };
            _dataMap.GetString(Worker.Jobs.Models.Constants.JobSettings).Returns(JsonConvert.SerializeObject(_jobSettings));

            var csvData = new CsvData { UserId = userId };
            _csvBlobStorage.GetAsync<CsvData>(Arg.Is("jobs"), Arg.Any<string>()).Returns(new[] { csvData });

            // Act
            await _sut.Execute(_context);

            // Assert
            var tasks = await Resolve<ITaskStorage>().GetAllAsync(userId, taskType);
            tasks.Should().BeEmpty();
        }

        [Theory]
        public async Task ShouldCreateOnlyAbsentTasks(ApplicationNoRelationsSpecimen appSpecimen,
                                                      (VerificationTaskNoRelationsSpecimen, VerificationTaskNoRelationsSpecimen) twoTasksSpecimen,
                                                      PolicyTasksSpecimen<UsPolicyTaskSpecimen> policyTasksSpecimen)
        {
            // Arrange
            appSpecimen.PolicyId = policyTasksSpecimen.Select(pt => pt.PolicyId).First();
            var existingTasks = new[] { twoTasksSpecimen.Item1, twoTasksSpecimen.Item2 };
            existingTasks.ForeachIndexed((t, i) => (t.VariantId, t.Type) = (policyTasksSpecimen[i].TaskVariantId, policyTasksSpecimen[i].TaskType));
            appSpecimen.RequiredTasks = existingTasks.ToArray();

            await Resolve<ApplicationFixture>().SaveRelationsAsync(appSpecimen);
            var application = new ApplicationBuilder().From(appSpecimen).Build();
            var applicationRepository = Resolve<IApplicationRepository>();
            await applicationRepository.SaveAsync(application);

            _jobSettings.TaskTypes = policyTasksSpecimen.Select(pt => pt.TaskType).ToArray();
            _dataMap.GetString(Worker.Jobs.Models.Constants.JobSettings).Returns(JsonConvert.SerializeObject(_jobSettings));

            var csvData = new CsvData { UserId = appSpecimen.UserId };
            _csvBlobStorage.GetAsync<CsvData>(Arg.Is("jobs"), Arg.Any<string>()).Returns(new[] { csvData });

            // Act
            await _sut.Execute(_context);

            // Assert
            var persistedApplication = await applicationRepository.GetAsync(application.Id);
            var applicationTasks = persistedApplication.RequiredTasks;

            var tasks = await Resolve<ITaskStorage>().GetAllAsync(application.UserId);
            tasks.Select(t => t.Id).Should().BeEquivalentTo(applicationTasks.Select(at => at.Id));
            tasks.Should().HaveSameCount(policyTasksSpecimen);
            tasks.Select(t => (t.VariantId, t.Type)).Should().BeEquivalentTo(policyTasksSpecimen.Select(pt => (pt.TaskVariantId, pt.TaskType)));
        }

        [Theory]
        public async Task ShouldNotCreateTask_WhenTaskOutOfPolicy(ApplicationNoRelationsSpecimen appSpecimen, GbPolicyTaskSpecimen policySpecimen)
        {
            // Arrange
            var taskType = TaskType.PepScreening;
            appSpecimen.PolicyId = policySpecimen.PolicyId;

            var application = new ApplicationBuilder().From(appSpecimen).Build();
            var applicationRepository = Resolve<IApplicationRepository>();
            await applicationRepository.SaveAsync(application);

            _jobSettings.TaskTypes = new[] { taskType };
            _dataMap.GetString(Worker.Jobs.Models.Constants.JobSettings).Returns(JsonConvert.SerializeObject(_jobSettings));

            var csvData = new CsvData { UserId = appSpecimen.UserId };
            _csvBlobStorage.GetAsync<CsvData>(Arg.Is("jobs"), Arg.Any<string>()).Returns(new[] { csvData });

            // Act
            await _sut.Execute(_context);

            // Assert
            var persistedApplication = await applicationRepository.GetAsync(application.Id);
            persistedApplication.RequiredTasks.Should().NotContain(at => at.TaskType == taskType);

            var tasks = await Resolve<ITaskStorage>().GetAllAsync(application.UserId, taskType);
            tasks.Should().HaveCount(0);
        }

        [Theory]
        public async Task ShouldCreateTask_WhenCsvHasDuplicates(ApplicationNoRelationsSpecimen appSpecimen, PolicyTaskSpecimen policySpecimen)
        {
            // Arrange
            var taskType = policySpecimen.TaskType;
            appSpecimen.PolicyId = policySpecimen.PolicyId;

            var application = new ApplicationBuilder().From(appSpecimen).Build();
            var applicationRepository = Resolve<IApplicationRepository>();
            await applicationRepository.SaveAsync(application);

            _jobSettings.TaskTypes = new[] { taskType };
            _jobSettings.ReadingBatchSize = 2;
            _dataMap.GetString(Worker.Jobs.Models.Constants.JobSettings).Returns(JsonConvert.SerializeObject(_jobSettings));

            var csvData = new CsvData { UserId = appSpecimen.UserId };
            _csvBlobStorage.GetAsync<CsvData>(Arg.Is("jobs"), Arg.Any<string>()).Returns(new[] { csvData, csvData });

            // Act
            await _sut.Execute(_context);

            // Assert
            var persistedApplication = await applicationRepository.GetAsync(application.Id);
            var applicationTasks = persistedApplication.RequiredTasks.Where(t => t.TaskType == taskType);
            var applicationTask = applicationTasks.Should().HaveCount(1).And.Subject.First();

            var tasks = await Resolve<ITaskStorage>().GetAllAsync(application.UserId, taskType);
            var task = tasks.Should().HaveCount(1).And.Subject.First();
            task.Id.Should().Be(applicationTask.Id);
            task.State.Should().Be(TaskState.Incomplete);
            task.VariantId.Should().Be(policySpecimen.TaskVariantId);
        }

        [Theory]
        public async Task ShouldCreateTask_WhenSettingsHasDuplicates(ApplicationNoRelationsSpecimen appSpecimen, PolicyTaskSpecimen policySpecimen)
        {
            // Arrange
            var taskType = policySpecimen.TaskType;
            appSpecimen.PolicyId = policySpecimen.PolicyId;

            var application = new ApplicationBuilder().From(appSpecimen).Build();
            var applicationRepository = Resolve<IApplicationRepository>();
            await applicationRepository.SaveAsync(application);

            _jobSettings.TaskTypes = new[] { taskType, taskType };
            _jobSettings.ReadingBatchSize = 2;
            _dataMap.GetString(Worker.Jobs.Models.Constants.JobSettings).Returns(JsonConvert.SerializeObject(_jobSettings));

            var csvData = new CsvData { UserId = appSpecimen.UserId };
            _csvBlobStorage.GetAsync<CsvData>(Arg.Is("jobs"), Arg.Any<string>()).Returns(new[] { csvData });

            // Act
            await _sut.Execute(_context);

            // Assert
            var persistedApplication = await applicationRepository.GetAsync(application.Id);
            var applicationTasks = persistedApplication.RequiredTasks.Where(t => t.TaskType == taskType);
            var applicationTask = applicationTasks.Should().HaveCount(1).And.Subject.First();

            var tasks = await Resolve<ITaskStorage>().GetAllAsync(application.UserId, taskType);
            var task = tasks.Should().HaveCount(1).And.Subject.First();
            task.Id.Should().Be(applicationTask.Id);
            task.State.Should().Be(TaskState.Incomplete);
            task.VariantId.Should().Be(policySpecimen.TaskVariantId);
        }

        [Theory]
        public async Task ShouldBeIdempotent(ApplicationNoRelationsSpecimen appSpecimen, PolicyTaskSpecimen policySpecimen)
        {
            // Arrange
            var taskType = policySpecimen.TaskType;
            appSpecimen.PolicyId = policySpecimen.PolicyId;

            var application = new ApplicationBuilder().From(appSpecimen).Build();
            var applicationRepository = Resolve<IApplicationRepository>();
            await applicationRepository.SaveAsync(application);

            _jobSettings.TaskTypes = new[] { taskType };
            _dataMap.GetString(Worker.Jobs.Models.Constants.JobSettings).Returns(JsonConvert.SerializeObject(_jobSettings));

            var csvData = new CsvData { UserId = appSpecimen.UserId };
            _csvBlobStorage.GetAsync<CsvData>(Arg.Is("jobs"), Arg.Any<string>()).Returns(new[] { csvData });

            // Act
            await _sut.Execute(_context);
            await _sut.Execute(_context);

            // Assert
            var persistedApplication = await applicationRepository.GetAsync(application.Id);
            var applicationTasks = persistedApplication.RequiredTasks.Where(t => t.TaskType == taskType);
            var applicationTask = applicationTasks.Should().HaveCount(1).And.Subject.First();

            var tasks = await Resolve<ITaskStorage>().GetAllAsync(application.UserId, taskType);
            var task = tasks.Should().HaveCount(1).And.Subject.First();
            task.Id.Should().Be(applicationTask.Id);
            task.State.Should().Be(TaskState.Incomplete);
            task.VariantId.Should().Be(policySpecimen.TaskVariantId);
        }

        [Theory]
        public async Task ShouldCreateTasks_WhenProcessManyOnePolicyItems(ApplicationNoRelationsSpecimen[] appSpecimens,
                                                                          PolicyTasksSpecimen<UsPolicyTaskSpecimen> policySpecimens)
        {
            // Arrange
            var applicationRepository = Resolve<IApplicationRepository>();

            foreach (var appSpecimen in appSpecimens)
            {
                appSpecimen.PolicyId = policySpecimens[0].PolicyId;
                var application = new ApplicationBuilder().From(appSpecimen).Build();
                await applicationRepository.SaveAsync(application);
            }

            _jobSettings.TaskTypes = policySpecimens.Select(p => p.TaskType).ToArray();
            _dataMap.GetString(Worker.Jobs.Models.Constants.JobSettings).Returns(JsonConvert.SerializeObject(_jobSettings));

            var csvData = appSpecimens.Select(s => new CsvData { UserId = s.UserId }).ToArray();
            _csvBlobStorage.GetAsync<CsvData>(Arg.Is("jobs"), Arg.Any<string>()).Returns(csvData);

            // Act
            await _sut.Execute(_context);

            // Assert
            foreach (var appSpecimen in appSpecimens)
            {
                foreach (var pt in policySpecimens)
                {
                    var persistedApplication = await applicationRepository.GetAsync(appSpecimen.Id);
                    var applicationTasks = persistedApplication.RequiredTasks.Where(t => t.TaskType == pt.TaskType);
                    var applicationTask = applicationTasks.Should().HaveCount(1).And.Subject.First();

                    var tasks = await Resolve<ITaskStorage>().GetAllAsync(appSpecimen.UserId, pt.TaskType);
                    var task = tasks.Should().HaveCount(1).And.Subject.First();
                    task.Id.Should().Be(applicationTask.Id);
                    task.State.Should().Be(TaskState.Incomplete);
                    task.VariantId.Should().Be(pt.TaskVariantId);
                }
            }
        }

        [Theory]
        public async Task ShouldCreateTasks_WhenProcessManyDifferentPolicyItems(ApplicationNoRelationsSpecimen[] appSpecimens,
                                                                                PolicyTasksSpecimen<IdentityPolicyTaskSpecimen> policySpecimens)
        {
            // Arrange
            var applicationRepository = Resolve<IApplicationRepository>();

            var taskType = policySpecimens.Select(pt => pt.TaskType).First();
            for (int i = 0; i < appSpecimens.Length; i++)
            {
                appSpecimens[i].PolicyId = policySpecimens[i % policySpecimens.Count].PolicyId;
                var application = new ApplicationBuilder().From(appSpecimens[i]).Build();
                await applicationRepository.SaveAsync(application);
            }

            _jobSettings.TaskTypes = new[] { taskType };
            _dataMap.GetString(Worker.Jobs.Models.Constants.JobSettings).Returns(JsonConvert.SerializeObject(_jobSettings));

            var csvData = appSpecimens.Select(s => new CsvData { UserId = s.UserId }).ToArray();
            _csvBlobStorage.GetAsync<CsvData>(Arg.Is("jobs"), Arg.Any<string>()).Returns(csvData);

            // Act
            await _sut.Execute(_context);

            // Assert
            foreach (var appSpecimen in appSpecimens)
            {
                var pt = policySpecimens.First(p => p.PolicyId == appSpecimen.PolicyId);
                var persistedApplication = await applicationRepository.GetAsync(appSpecimen.Id);
                var applicationTasks = persistedApplication.RequiredTasks.Where(t => t.TaskType == taskType);
                var applicationTask = applicationTasks.Should().HaveCount(1).And.Subject.First();

                var tasks = await Resolve<ITaskStorage>().GetAllAsync(appSpecimen.UserId, taskType);
                var task = tasks.Should().HaveCount(1).And.Subject.First();
                task.Id.Should().Be(applicationTask.Id);
                task.State.Should().Be(TaskState.Incomplete);
                task.VariantId.Should().Be(pt.TaskVariantId);
            }
        }
    }
}
