using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using FluentAssertions.Execution;
using FsCheck;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Quartz;
using Serilog.Core;
using WX.B2C.User.Verification.BlobStorage.Storages;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries;
using WX.B2C.User.Verification.Integration.Tests.Builders;
using WX.B2C.User.Verification.Integration.Tests.Fixtures;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.IoC;
using WX.B2C.User.Verification.Worker.Jobs.Jobs;
using WX.B2C.User.Verification.Worker.Jobs.Providers;
using WX.B2C.User.Verification.Worker.Jobs.Settings;
using static WX.B2C.User.Verification.Worker.Jobs.Providers.InstructChecksDataProvider;
using Check = WX.B2C.User.Verification.Domain.Models.Check;
using IJobTaskRepository = WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories.ITaskRepository;
using IJobCheckRepository = WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories.ICheckRepository;

namespace WX.B2C.User.Verification.Integration.Tests.Jobs
{
    internal class InstructChecksJobTests : BaseIntegrationTest
    {
        private InstructChecksJob _sut;
        private IJobExecutionContext _context;
        
        private IEnumerable<CheckVariantSpecimen> _policyChecks;
        private CsvData[] _csvData;
        
        private VerificationTaskFixture _taskFixture;
        private ITaskRepository _taskRepository;
        private ApplicationFixture _applicationFixture;
        private List<Guid> _users = new();

        protected override void RegisterModules(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ApplicationFixture>().AsSelf().SingleInstance();
            containerBuilder.RegisterType<VerificationTaskFixture>().AsSelf().SingleInstance();
            containerBuilder.RegisterDbQueryFactory()
                            .RegisterJobRepositories();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            
            Arb.Register<VerificationTaskArbitrary>();
            Arb.Register<ApplicationArbitrary>();
            Arb.Register<ApplicationNoRelationsArbitrary>();
            Arb.Register<VerificationTaskNoRelationsArbitrary>();
            Arb.Register<CheckVariantArbitrary>();
            Arb.Register<ArrayArbitrary<VerificationTaskNoRelationsSpecimen>>();
            Arb.Register<ArrayArbitrary<Guid>>();
            Arb.Register<ArrayArbitrary<CheckVariantSpecimen>>();
            Arb.Register<ArrayArbitrary<(VerificationTaskNoRelationsSpecimen, Guid[], Guid[])>>();

            _taskFixture = Resolve<VerificationTaskFixture>();
            _applicationFixture = Resolve<ApplicationFixture>();
            _taskRepository = Resolve<ITaskRepository>();
        }

        [SetUp]
        public void Setup()
        {
            var jobSettings = new InstructChecksJobSettings
            {
                ProcessBatchSize = 3,
                ReadingBatchSize = 10,
                DelayInMillisecondsAfterBatch = 0,
                MaxErrorCount = 0,
                FileName = string.Empty,
                DirectUpdate = true
            };
            
            var dataMap = Substitute.For<JobDataMap>();
            dataMap.GetString(Worker.Jobs.Models.Constants.JobSettings).Returns(JsonConvert.SerializeObject(jobSettings));
            
            _context = Substitute.For<IJobExecutionContext>();
            _context.MergedJobDataMap.Returns(_ => dataMap);

            var csvBlobStorage = Substitute.For<ICsvBlobStorage>();
            csvBlobStorage.GetAsync<CsvData>(Arg.Is("jobs"), Arg.Any<string>()).Returns(_ => _csvData);
            var dataProvider = new InstructChecksDataProvider(Resolve<IQueryFactory>(), csvBlobStorage);
            
            var verificationPolicyStorage = Substitute.For<IVerificationPolicyStorage>();
            verificationPolicyStorage.GetChecksInfoAsync()
                                     .Returns(_ => Task.FromResult(_policyChecks.Select(variantSpecimen => new CheckVariantInfo
                                                                                {
                                                                                    Id = variantSpecimen.Id,
                                                                                    Provider = variantSpecimen.Provider,
                                                                                    Type = variantSpecimen.Type,
                                                                                    MaxAttempts = variantSpecimen.MaxAttempts
                                                                                })
                                                                                .ToArray()));
            
            _sut = new InstructChecksJob(Substitute.For<ICheckService>(),
                                         Resolve<IJobTaskRepository>(),
                                         Resolve<IJobCheckRepository>(),
                                         verificationPolicyStorage,
                                         dataProvider,
                                         Logger.None);
        }
        
        [OneTimeTearDown]
        public Task OneTimeTearDown() => DbFixture.Clear(_users.ToArray());

        [Theory]
        public async Task ShouldNotAddChecksToTask_WhenApplicationIsNotCreated(VerificationTaskNoRelationsSpecimen taskSpecimen,
                                                                               CheckVariantSpecimen[] newChecks)
        {
            // Arrange
            _users.Add(taskSpecimen.UserId);
            _policyChecks = newChecks;
            var acceptanceChecks = newChecks.Select(variantSpecimen => variantSpecimen.Id).ToArray();

            var task = new VerificationTaskBuilder().From(taskSpecimen).Build();
            var taskRepository = Resolve<ITaskRepository>();
            await taskRepository.SaveAsync(task);

            var csvData = new CsvData
            {
                UserId = taskSpecimen.UserId, TaskType = taskSpecimen.Type,
                AcceptanceChecks = acceptanceChecks
            };
            _csvData = new[] { csvData };

            // Act
            await _sut.Execute(_context);

            // Assert
            var persistedTask = await taskRepository.GetAsync(task.Id);
            persistedTask.Checks.Select(x => x.VariantId).Should().BeEmpty();
        }

        [Theory]
        public async Task ShouldAddChecksToTask(ApplicationNoRelationsSpecimen applicationSpecimen,
                                                VerificationTaskNoRelationsSpecimen taskSpecimen,
                                                CheckVariantSpecimen[] newChecks)
        {
            // Arrange
            _users.Add(applicationSpecimen.UserId);
            _policyChecks = newChecks;
            var acceptanceChecks = newChecks.Select(variantSpecimen => variantSpecimen.Id).ToArray();

            applicationSpecimen.RequiredTasks = new[] { taskSpecimen };
            await _applicationFixture.SaveAsync(applicationSpecimen);
            
            var csvData = new CsvData
            {
                UserId = applicationSpecimen.UserId, TaskType = taskSpecimen.Type,
                AcceptanceChecks = acceptanceChecks
            };
            _csvData = new[] { csvData };

            // Act
            await _sut.Execute(_context);

            // Assert
            var persistedTask = await _taskRepository.GetAsync(taskSpecimen.Id);
            persistedTask.Checks.Select(x => x.VariantId).Should().BeEquivalentTo(acceptanceChecks);
        }

        [Theory]
        public async Task ShouldAddChecksToTask_WhenCsvHasDuplicates(ApplicationNoRelationsSpecimen applicationSpecimen,
                                                                     VerificationTaskNoRelationsSpecimen taskSpecimen,
                                                                     CheckVariantSpecimen[] newChecks)
        {
            // Arrange
            _users.Add(applicationSpecimen.UserId);
            _policyChecks = newChecks;
            var acceptanceChecks = newChecks.Select(variantSpecimen => variantSpecimen.Id).ToArray();

            applicationSpecimen.RequiredTasks = new[] { taskSpecimen };
            await _applicationFixture.SaveAsync(applicationSpecimen);

            var csvData = new CsvData { UserId = applicationSpecimen.UserId, TaskType = taskSpecimen.Type, AcceptanceChecks = acceptanceChecks };
            _csvData = new[] { csvData, csvData };

            // Act
            await _sut.Execute(_context);

            // Assert
            var persistedTask = await _taskRepository.GetAsync(taskSpecimen.Id);
            persistedTask.Checks.Select(x => x.VariantId).Should().BeEquivalentTo(acceptanceChecks);
        }

        [Theory]
        public async Task ShouldNotAddCheckToTask_WhenItAlreadyExists(ApplicationNoRelationsSpecimen applicationSpecimen,
                                                                      VerificationTaskNoRelationsSpecimen taskSpecimen,
                                                                      CheckVariantSpecimen checkVariant)
        {
            //Given
            _users.Add(applicationSpecimen.UserId);
            taskSpecimen.PerformedChecks = new HashSet<TaskCheck>
            {
                TaskCheck.Create(Check.Create(Guid.NewGuid(),
                                              taskSpecimen.UserId,
                                              checkVariant.Type,
                                              CheckVariant.Create(checkVariant.Id, checkVariant.Provider),
                                              new[] { taskSpecimen.Id },
                                              new Initiation("test", "test")))
            };

            _policyChecks = new[] { checkVariant };

            var task = new VerificationTaskBuilder().From(taskSpecimen).Build();
            applicationSpecimen.RequiredTasks = new[] { taskSpecimen };
            await _applicationFixture.SaveAsync(applicationSpecimen);
            await _taskFixture.SaveChecks(task);
            
            // Arrange
            var csvData = new CsvData { UserId = taskSpecimen.UserId, TaskType = taskSpecimen.Type, AcceptanceChecks = new[] { checkVariant.Id } };
            _csvData = new[] { csvData };

            var existingAcceptanceChecks = task.Checks.Select(check => check.VariantId);

            // Act
            await _sut.Execute(_context);

            // Assert
            var persistedTask = await _taskRepository.GetAsync(task.Id);
            persistedTask.Checks.Select(x => x.VariantId).Should().BeEquivalentTo(existingAcceptanceChecks);
            persistedTask.Checks.Should().HaveCount(1);
        }

        [Theory]
        public async Task ShouldAddOnlyAbsentChecksToTask(ApplicationNoRelationsSpecimen applicationSpecimen,
                                                          VerificationTaskSpecimen taskSpecimen,
                                                          Guid[] newChecks)
        {
            //Given
            var task = new VerificationTaskBuilder().From(taskSpecimen).Build();
            await _taskFixture.SaveCollectionSteps(task);
            applicationSpecimen.UserId = task.UserId;
            applicationSpecimen.RequiredTasks = new[] { taskSpecimen };
            await _applicationFixture.SaveAsync(applicationSpecimen);
            await _taskFixture.SaveChecks(task);
        
            var existingChecks = task.Checks.Select(check => check.VariantId);
            var allChecks = existingChecks.Concat(newChecks).Distinct().ToArray();
            
            _users.Add(applicationSpecimen.UserId);
            _policyChecks = allChecks.Select(variantId => new CheckVariantSpecimen
            {
                Id = variantId, Provider = CheckProviderType.System, Type = CheckType.IpMatch, MaxAttempts = 0
            });
            
            //Arrange
            var csvData = new CsvData { UserId = task.UserId, TaskType = task.Type, AcceptanceChecks = allChecks };
            _csvData = new[] { csvData };
        
            // Act
            await _sut.Execute(_context);
        
            // Assert
            var persistedTask = await _taskRepository.GetAsync(task.Id);
            persistedTask.Checks.Select(x => x.VariantId).Should().BeEquivalentTo(allChecks);
        }
        
        [Theory]
        public async Task ShouldBeIdempotent(ApplicationNoRelationsSpecimen applicationSpecimen,
                                             VerificationTaskSpecimen taskSpecimen,
                                             Guid[] newChecks)
        {
            //Given
            var task = new VerificationTaskBuilder().From(taskSpecimen).Build();
            await _taskFixture.SaveCollectionSteps(task);
            applicationSpecimen.UserId = task.UserId;
            applicationSpecimen.RequiredTasks = new[] { taskSpecimen };
            await _applicationFixture.SaveAsync(applicationSpecimen);
            await _taskFixture.SaveChecks(task);
        
            var existingChecks = task.Checks.Select(check => check.VariantId);
            var allChecks = existingChecks.Concat(newChecks).Distinct().ToArray();
            
            _users.Add(applicationSpecimen.UserId);
            _policyChecks = allChecks.Select(variantId => new CheckVariantSpecimen
            {
                Id = variantId, Provider = CheckProviderType.System, Type = CheckType.IpMatch, MaxAttempts = 0
            });
            
            // Arrange
            var csvData = new CsvData { UserId = taskSpecimen.UserId, TaskType = taskSpecimen.Type, AcceptanceChecks = allChecks };
            _csvData = new[] { csvData };
        
            // Act
            await _sut.Execute(_context);
            await _sut.Execute(_context);
        
            // Assert
            var persistedTask = await _taskRepository.GetAsync(task.Id);
            persistedTask.Checks.Select(x => x.VariantId).Should().BeEquivalentTo(allChecks);
        }
        
        [Theory]
        public async Task ShouldAddChecksToTask_WhenProcessManyItems((ApplicationNoRelationsSpecimen applicationSpecimen, VerificationTaskSpecimen Specimen, Guid[] NewChecks)[] testData)
        {
            // Given
            _policyChecks = testData.SelectMany(tuple => tuple.NewChecks.Select(variantId => new CheckVariantSpecimen
                                    {
                                        Id = variantId, Provider = CheckProviderType.System, Type = CheckType.IpMatch
                                    }))
                                    .ToArray();
            
            foreach (var (applicationSpecimen, taskSpecimen, _) in testData)
            {
                taskSpecimen.CollectionSteps.Clear();
                applicationSpecimen.UserId = taskSpecimen.UserId;
                applicationSpecimen.RequiredTasks = new[] { taskSpecimen };
                await _applicationFixture.SaveAsync(applicationSpecimen);
                var task = new VerificationTaskBuilder().From(taskSpecimen).Build();
                await _taskFixture.SaveChecks(task);
                
                _users.Add(applicationSpecimen.UserId);
            }
        
            // Arrange
            _csvData = testData.Select(t => new CsvData
            {
                UserId = t.Specimen.UserId, TaskType = t.Specimen.Type, AcceptanceChecks = t.NewChecks
            }).ToArray();
        
            // Act
            await _sut.Execute(_context);
        
            // Assert
            using var _ = new AssertionScope();
            foreach (var (_ , taskSpecimen, checks) in testData)
            {
                var persistedTask = await _taskRepository.GetAsync(taskSpecimen.Id);
                var expected = taskSpecimen.PerformedChecks.Select(check => check.VariantId).Concat(checks);
                var actual = persistedTask.Checks.Select(x => x.VariantId);
                actual.Should().BeEquivalentTo(expected);
            }
        }
        
        [Theory]
        public async Task ShouldAddChecksToTask_WhenOneUserHaveManyDifferentTasks(ApplicationNoRelationsSpecimen applicationSpecimen,
                                                                                  (VerificationTaskSpecimen Specimen, Guid[] NewChecks)[] testData)
        {
            // Given
            _policyChecks = testData.SelectMany(tuple => tuple.NewChecks.Select(variantId => new CheckVariantSpecimen
                                    {
                                        Id = variantId, Provider = CheckProviderType.System, Type = CheckType.IpMatch
                                    }))
                                    .ToArray();
        
            await _applicationFixture.SaveAsync(applicationSpecimen);
            _users.Add(applicationSpecimen.UserId);
            
            var createdTasks = new Dictionary<TaskType, (VerificationTask Task, Guid[] Checks)>();
            foreach (var tuple in testData)
            {
                if (createdTasks.ContainsKey(tuple.Specimen.Type))
                    continue;
        
                tuple.Specimen.UserId = applicationSpecimen.UserId;
                tuple.Specimen.CollectionSteps.Clear();
                var task = new VerificationTaskBuilder().From(tuple.Specimen).Build();
                await _taskRepository.SaveAsync(task);
                await _taskFixture.SaveChecks(task);
        
                createdTasks.Add(task.Type, (task, tuple.NewChecks));
            }
        
            // Arrange
            _csvData = createdTasks.Values
                                   .Select(t => new CsvData { UserId = t.Task.UserId, TaskType = t.Task.Type, AcceptanceChecks = t.Checks })
                                   .ToArray();
        
            // Act
            await _sut.Execute(_context);
        
            // Assert
            using var _ = new AssertionScope();
            foreach (var (task, checks) in createdTasks.Values)
            {
                var persistedTask = await _taskRepository.GetAsync(task.Id);
                var expected = task.Checks.Select(check => check.VariantId).Concat(checks).Distinct().ToArray();
                var actual = persistedTask.Checks.Select(x => x.VariantId).ToArray();
                actual.Should().BeEquivalentTo(expected.Distinct());
            }
        }
    }
}