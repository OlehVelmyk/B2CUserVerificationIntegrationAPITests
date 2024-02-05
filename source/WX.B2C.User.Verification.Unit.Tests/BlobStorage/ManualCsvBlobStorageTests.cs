using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Serilog.Core;
using WX.B2C.User.Verification.BlobStorage.Factories;
using WX.B2C.User.Verification.BlobStorage.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Infrastructure.Common.Configuration;

namespace WX.B2C.User.Verification.Unit.Tests.BlobStorage
{

    /// <summary>
    /// Tests to check faster file created by Roman before using it in prod.
    /// </summary>
    internal class ManualCsvBlobStorageTests
    {
        private ICsvBlobStorage _sut;
        private IBlobStorage _blobStorage;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _blobStorage = Substitute.For<IBlobStorage>();
            _sut = new CsvBlobStorage(_blobStorage);
        }

        [Test]
        [Explicit]
        public async Task ShouldParseFile()
        {
            // Arrange
            var path = @""; //Put path to your file here
            using var stream = (Stream) new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            _blobStorage.DownloadAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult(stream));

            // Act
            var result = await _sut.GetAsync<CsvData>(null, null);

            // Assert
            var any = result.Any(data => data.AcceptanceChecks == null);
        }

        [Test]
        [Explicit]
        public async Task ShouldParseFileFromBlob()
        {
            // Arrange
            _blobStorage = new Verification.BlobStorage.Storages.BlobStorage(new AppLocalConfig(), new BlobContainerClientFactory(), Logger.None);
            _sut = new CsvBlobStorage(_blobStorage);

            // Act
            var result = await _sut.GetAsync<CsvData>("jobs", ""); //Put file here

            // Assert
            var any = result.Any(data => data.AcceptanceChecks == null);
        }

        /// <summary>
        /// Put properties of your file here
        /// </summary>
        public class CsvData
        {
            public Guid UserId { get; set; }

            public TaskType TaskType { get; set; }

            public Guid[] AcceptanceChecks { get; set; }
        }
    }
}