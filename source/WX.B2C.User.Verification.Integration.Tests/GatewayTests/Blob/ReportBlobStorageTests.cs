using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.BlobStorage.Factories;
using WX.B2C.User.Verification.BlobStorage.IoC;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.Core.TypeExtensions;

namespace WX.B2C.User.Verification.Integration.Tests.GatewayTests.Blob
{
    [TestFixture]
    public class ReportBlobStorageTests : BaseIntegrationTest
    {
        private string _containerName;

        protected override void RegisterModules(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterBlobStorage();
        }

        [SetUp]
        public void Setup()
        {
            _containerName = $"wx-b2c-user-verification-int-test-{DateTime.Now:HH-mm-ss}";
        }

        [TearDown]
        public async Task CleanUpContainer()
        {
            var clientFactory = Resolve<IBlobContainerClientFactory>();
            var appConfig = Resolve<IAppConfig>();

            var client = await clientFactory.CreateAsync(appConfig.StorageConnectionString.UnSecure(), _containerName);
            await client.DeleteIfExistsAsync();
        }

        [Test]
        public async Task ShouldSaveReport()
        {
            var storage = Resolve<IReportsBlobStorage>();
            await using var reportPart = ToStream("Test data");

            Func<Task> act = () => storage.AppendAsync(_containerName, "ShouldSaveReport", reportPart);

            await act.Should().NotThrowAsync();
        }

        [Test]
        public async Task ShouldSaveReport_WhenReportHasMultipleParts()
        {
            var storage = Resolve<IReportsBlobStorage>();
            var parts = new List<string>();
            for (var i = 0; i < 10; i++)
            {
                parts.Add($"Test data {i}{Environment.NewLine}");
            }

            Func<Task> act = async () =>
            {
                foreach (var part in parts)
                {
                    await using var reportPart = ToStream(part);
                    await storage.AppendAsync(_containerName, "ShouldSaveReport_WhenReportHasMultipleParts", reportPart);
                }
            };

            await act.Should().NotThrowAsync();
        }
        
        private static Stream ToStream(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value));
        }
    }
}