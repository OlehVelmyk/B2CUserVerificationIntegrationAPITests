using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using FsCheck;
using NSubstitute;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Infrastructure.Common.Configuration;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Providers;
using WX.B2C.User.Verification.Worker.Jobs.Settings;
using WX.Core.TypeExtensions;

namespace WX.B2C.User.Verification.Integration.Tests.Jobs.Providers
{
    [TestFixture]
    public class FileDataProviderTests : BaseIntegrationTest
    {
        private IFileRepository _fileRepository;
        private bool _shouldUseEnv = false;
        private FileDataProvider _provider;

        protected override void RegisterModules(ContainerBuilder containerBuilder)
        {
            base.RegisterModules(containerBuilder);
            RegisterKeyVault<IB2CUserVerificationKeyVault>(containerBuilder);

            containerBuilder.Register(_ =>
            {
                var connectionString = Resolve<IB2CUserVerificationKeyVault>().DbConnectionString.UnSecure();
                var appConfig = Substitute.For<IAppConfig>();
                appConfig.DbConnectionString.Returns(_ => _shouldUseEnv ? connectionString.Secure() : new AppLocalConfig().DbConnectionString);

                return appConfig;
            }).As<IAppConfig>().SingleInstance();


            containerBuilder.RegisterDbQueryFactory();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _fileRepository = Resolve<IFileRepository>();
            Arb.Register<FileArbitrary>();

            _provider = new FileDataProvider(Resolve<IQueryFactory>());
        }

        [Test]
        public async Task ShouldCountBeZero_WhenNoFilesExistsByCriteria()
        {
            _shouldUseEnv = true;
           
            var settings = new FileValidationJobSettings
            {
                Files = new[] { Guid.NewGuid() }
            };
            var count = await _provider.GetTotalCountAsync(settings, default);

            count.Should().Be(0);
        }

        [Test]
        public async Task ShouldReadCount_WhenUsedDevDatabase()
        {
            _shouldUseEnv = true;

            var settings = new FileValidationJobSettings();
            var count = await _provider.GetTotalCountAsync(settings, default);

            count.Should().BeGreaterThan(0);
        }

        [Theory(StartSize = 50)]
        public async Task ShouldBatches(NonEmptyArray<FileDto> files, PositiveInt size, PositiveInt count)
        {
            // Given
            var batchSize = size.Get % files.Get.Length + 1;
            var expectedFilesCount = count.Get % files.Get.Length + 1;
            var expectedBatchCount = (expectedFilesCount - 1) / batchSize + 1;

            await files.Get.Foreach(_fileRepository.SaveAsync);
            var userIds = files.Get.Select(file => file.UserId).Take(expectedFilesCount);

            // Arrange
            var expectedFiles = files.Get.Where(file => file.UserId.In(userIds))
                                     .OrderBy(file => file.Id)
                                     .ToArray();

            var settings = new FileValidationJobSettings
            {
                Users = userIds.ToArray(),
                ReadingBatchSize = batchSize
            };

            var totalCount = await _provider.GetTotalCountAsync(settings, default);

            var batchCount = 0;
            var actualFiles = new List<FileData>();
            await foreach (var batch in _provider.GetAsync(settings, default))
            {
                batchCount++;
                actualFiles.AddRange(batch);
            }

            totalCount.Should().Be(expectedFiles.Length);
            batchCount.Should().Be(expectedBatchCount);
            expectedFiles.Should()
                         .BeEquivalentTo(actualFiles.ToArray(),
                                         option => option.ExcludingMissingMembers());
        }
    }
}