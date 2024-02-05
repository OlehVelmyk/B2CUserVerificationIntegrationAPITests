using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries;

namespace WX.B2C.User.Verification.Integration.Tests.DataAccess
{
    class FileStorageTests : BaseIntegrationTest
    {
        private readonly List<Guid> _users = new();

        private IFileStorage _sut;
        private IFileRepository _fileRepository;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _sut = Resolve<IFileStorage>();
            _fileRepository = Resolve<IFileRepository>();
            Arb.Register<FileArbitrary>();
        }

        [OneTimeTearDown]
        public Task OneTimeTearDown() => DbFixture.Clear(_users.ToArray());

        [Theory]
        public async Task IFileStorage_GetAsync_ShouldFindFile(FileDto expected)
        {
            // Given
            _users.Add(expected.UserId);
            await _fileRepository.SaveAsync(expected);

            // Act
            var actual = await _sut.GetAsync(expected.Id);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        public async Task IFileStorage_GetAsync_ShouldFindFiles(FileDto expected)
        {
            // Given
            _users.Add(expected.UserId);
            await _fileRepository.SaveAsync(expected);

            // Act
            var actual = await _sut.FindAsync(expected.UserId, expected.Id);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
