using System;
using System.IO;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using WX.B2C.User.Verification.BlobStorage.Factories;
using WX.B2C.User.Verification.BlobStorage.IoC;
using WX.B2C.User.Verification.Configuration;
using WX.B2C.User.Verification.Configuration.IoC;
using WX.B2C.User.Verification.Configuration.Models;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults;
using WX.Configuration.Contracts;
using WX.Core.TypeExtensions;

namespace WX.B2C.User.Verification.Integration.Tests.Configuration
{
    [TestFixture]
    [Explicit]
    public class ConfigurationSetupUtilities : BaseIntegrationTest
    {
        protected override void RegisterModules(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterBlobStorage();
            RegisterKeyVault<IMessagingKeyVault>(containerBuilder);
        }

        [Test]
        [Explicit]
        public async Task SetupPhoneCodes() => await SetupBlobAsync("phone-codes.json", "PhoneCodes.json");

        [Test]
        [Explicit]
        public async Task SetupExcludedNames() => await SetupBlobAsync("excluded-names.json", "ExcludedNames.json");

        public async Task SetupBlobAsync(string blobName, string seedFileName)
        {
            // TODO: Some countries can have multiple phone codes but only one listed
            // PR: [1787,1939]
            // DO: [1809,1829,1849]

            const string containerName = "b2c";
            string blobPath = $"configuration/{blobName}";
            var pathToSeed = Path.Combine(Directory.GetCurrentDirectory(), "Configuration", "Source", seedFileName);

            var messagingKeyVault = Resolve<IMessagingKeyVault>();
            var containerClientFactory = Resolve<IBlobContainerClientFactory>();
            var containerClient = await containerClientFactory.CreateAsync(messagingKeyVault.B2CStorageConnectionString.UnSecure(), containerName);
            var blobClient = containerClient.GetBlobClient(blobPath);

            _ = await blobClient.DeleteIfExistsAsync();
            var response = await blobClient.UploadAsync(pathToSeed);
            response.GetRawResponse().Status.Should().Be(StatusCodes.Status201Created);
        }
    }
}