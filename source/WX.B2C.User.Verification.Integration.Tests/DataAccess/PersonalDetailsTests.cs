using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Equivalency;
using FluentAssertions.Extensions;
using FsCheck;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.DataAccess.Entities;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries;

namespace WX.B2C.User.Verification.Integration.Tests.DataAccess
{
    [TestFixture]
    public class PersonalDetailsTests : BaseIntegrationTest
    {
        private readonly List<Guid> _users = new();

        private IPersonalDetailsRepository _repository;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _repository = Resolve<IPersonalDetailsRepository>();

            Arb.Register<PersonalDetailsArbitrary>();
        }

        [OneTimeTearDown]
        public Task OneTimeTearDown() => DbFixture.Clear(_users.ToArray());

        [Theory]
        public async Task ShouldCreatePersonalDetails(PersonalDetailsDto personalDetailsDto)
        {
            // Act
            _users.Add(personalDetailsDto.UserId);
            await _repository.SaveAsync(personalDetailsDto);

            // Assert
            var entity = await DbFixture.DbContext.Set<PersonalDetails>()
                                  .Include(details => details.ResidenceAddress)
                                  .FirstOrDefaultAsync(details => details.UserId == personalDetailsDto.UserId);

            entity.Should().NotBeNull();
            entity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, 1.Seconds());
            personalDetailsDto.Should().BeEquivalentTo(
                entity, 
                config => config.ExcludingMissingMembers().Excluding(d => d.CreatedAt));
        }

        [Theory]
        public async Task ShouldFindPersonalDetails(PersonalDetailsDto personalDetailsDto)
        {
            // Arrange
            _users.Add(personalDetailsDto.UserId);
            await _repository.SaveAsync(personalDetailsDto);

            // Act
            var actual = await _repository.FindAsync(personalDetailsDto.UserId);

            // Assert
            actual.Should().BeEquivalentTo(personalDetailsDto, _assertionSettings);
        }

        [Theory]
        public async Task ShouldUpdatePersonalDetails(PersonalDetailsDto old, PersonalDetailsDto @new)
        {
            // Arrange
            @new.UserId = old.UserId;
            _users.Add(old.UserId);
            await _repository.SaveAsync(old);

            // Act
            await _repository.SaveAsync(@new);
            var actual = await _repository.FindAsync(@new.UserId);

            // Assert
            actual.Should().BeEquivalentTo(@new, _assertionSettings);
        }

        [Theory]
        public async Task ShouldUpdatePersonalDetails_WhenOnlyAddressChanged(PersonalDetailsDto old, PersonalDetailsDto @new)
        {
            // Arrange
            @new.UserId = old.UserId;
            old.ResidenceAddress = null;
            _users.Add(old.UserId);
            await _repository.SaveAsync(old);

            // Act
            await _repository.SaveAsync(@new);
            var actual = await _repository.FindAsync(@new.UserId);

            // Assert
            actual.Should().BeEquivalentTo(@new, _assertionSettings);
        }

        private readonly Func<EquivalencyAssertionOptions<PersonalDetailsDto>, EquivalencyAssertionOptions<PersonalDetailsDto>>
            _assertionSettings = options =>
            {
                return options.Using<DateTime>(context => context.Subject.Should().BeCloseTo(DateTime.UtcNow, 1.Seconds()))
                              .When(info => info.Path.Contains(nameof(AuditableEntity.CreatedAt)));
            };
    }
}