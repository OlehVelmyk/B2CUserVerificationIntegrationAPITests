using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.DataAccess.Entities;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries;

namespace WX.B2C.User.Verification.Integration.Tests.DataAccess
{
    [TestFixture]
    public class VerificationDetailsTests : BaseIntegrationTest
    {
        private readonly List<Guid> _users = new();

        private IVerificationDetailsRepository _repository;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Arb.Register<VerificationDetailsArbitrary>();
            Arb.Register<TaxResidenceArbitrary>();
            _repository = Resolve<IVerificationDetailsRepository>();
        }

        [OneTimeTearDown]
        public Task OneTimeTearDown() => DbFixture.Clear(_users.ToArray());

        [Theory]
        public async Task ShouldCreateVerificationDetails(VerificationDetailsDto verificationDetailsDto)
        {
            _users.Add(verificationDetailsDto.UserId);

            // Act
            await _repository.SaveAsync(verificationDetailsDto);

            // Assert
            var entity = DbFixture.DbContext.Find<VerificationDetails>(verificationDetailsDto.UserId);
            entity.Should().NotBeNull();
            verificationDetailsDto.Should().BeEquivalentTo(entity, options => options.ExcludingMissingMembers()
                                                                                     .Excluding(e => e.IdDocumentNumber)
                                                                                     .Excluding(e => e.IdDocumentNumberType));
            verificationDetailsDto.IdDocumentNumber.Number.Should().Be(entity.IdDocumentNumber);
            verificationDetailsDto.IdDocumentNumber.Type.Should().Be(entity.IdDocumentNumberType);
        }

        [Theory]
        public async Task ShouldFindVerificationDetails(VerificationDetailsDto verificationDetailsDto)
        {
            // Given
            _users.Add(verificationDetailsDto.UserId);
            await _repository.SaveAsync(verificationDetailsDto);

            // Act
            var actual = await _repository.FindAsync(verificationDetailsDto.UserId);

            // Assert
            actual.Should().BeEquivalentTo(verificationDetailsDto);
        }

        [Theory]
        public async Task ShouldUpdateVerificationDetails(VerificationDetailsDto old, VerificationDetailsDto @new)
        {
            // Arrange
            @new.UserId = old.UserId;
            _users.Add(old.UserId);
            await _repository.SaveAsync(old);

            // Act
            await _repository.SaveAsync(@new);
            var actual = await _repository.FindAsync(@new.UserId);

            // Assert
            actual.Should().BeEquivalentTo(@new);
        }
    }
}