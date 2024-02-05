using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FsCheck;
using FluentAssertions;
using NUnit.Framework;
using Optional.Unsafe;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries;

namespace WX.B2C.User.Verification.Integration.Tests.DataAccess
{
    class FileRepositoryTests : BaseIntegrationTest
    {
        private readonly List<Guid> _users = new();

        private IFileRepository _sut;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _sut = Resolve<IFileRepository>();
            Arb.Register<FileArbitrary>();
        }

        [OneTimeTearDown]
        public Task OneTimeTearDown() => DbFixture.Clear(_users.ToArray());

        [Theory]
        public async Task SaveAsync_ShouldSaveFile(FileDto expected)
        {
            _users.Add(expected.UserId);

            // Act 
            await _sut.SaveAsync(expected);

            // Assert
            var actual = DbFixture.DbContext.Find<Verification.DataAccess.Entities.File>(expected.Id);
            actual.Should().BeEquivalentTo(expected, options => options.Excluding(dto => dto.Crc32Checksum));
            actual.Crc32Checksum.HasValue.Should().Be(expected.Crc32Checksum.HasValue);
            actual.Crc32Checksum.GetValueOrDefault().Should().Be(expected.Crc32Checksum.ValueOrDefault());
        }

        [Theory]
        public async Task GetAsync_ShouldFindFile(FileDto expected)
        {
            // Given
            _users.Add(expected.UserId);
            await _sut.SaveAsync(expected);

            // Act
            var actual = await _sut.GetAsync(expected.Id);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        public async Task SaveAsync_ShouldUpdateFile(FileDto old, FileDto @new)
        {
            // Given
            _users.Add(old.UserId);
            await _sut.SaveAsync(old);
            var saved = await _sut.GetAsync(old.Id);
            saved.Should().BeEquivalentTo(old);

            // Arrange
            (@new.Id, @new.UserId) = (old.Id, old.UserId);

            // Act
            await _sut.SaveAsync(@new);

            // Assert
            var actual = await _sut.GetAsync(@new.Id);
            actual.Should().BeEquivalentTo(@new);
        }
    }
}
