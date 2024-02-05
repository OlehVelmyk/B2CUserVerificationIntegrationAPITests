using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using NSubstitute;
using NUnit.Framework;
using Optional;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.Core.Services;
using WX.B2C.User.Verification.Core.Services.Mappers;
using WX.B2C.User.Verification.Domain.Events;
using WX.B2C.User.Verification.Domain.Profile;
using WX.B2C.User.Verification.Domain.Shared;
using WX.B2C.User.Verification.Unit.Tests.Extensions;

namespace WX.B2C.User.Verification.Unit.Tests.Services
{
    [TestFixture]
    public class ProfileServiceTests
    {
        private IProfileService _profileService;
        private IPersonalDetailsRepository _personalDetailsRepository;
        private IVerificationDetailsRepository _verificationDetailsRepository;
        private IEventPublisher _eventPublisher;

        [SetUp]
        public void Setup()
        {
            var patcher = new ProfilePatcher();
            _personalDetailsRepository = Substitute.For<IPersonalDetailsRepository>();
            _verificationDetailsRepository = Substitute.For<IVerificationDetailsRepository>();
            _eventPublisher = Substitute.For<IEventPublisher>();
            _profileService = new ProfileService(_personalDetailsRepository,
                                                 _verificationDetailsRepository,
                                                 Substitute.For<IExternalProfileRepository>(),
                                                 patcher,
                                                 Substitute.For<IInitiationMapper>(),
                                                 _eventPublisher);
        }

        [Theory]
        public async Task ShouldUpdateVerificationDetails_WhenIpAddressHasChanged(VerificationDetailsDto verificationDetailsDto,
                                                                                  NonEmptyString newIpAddress)
        {
            // Arrange
            var userId = verificationDetailsDto.UserId;
            _verificationDetailsRepository.FindAsync(userId)
                                          .Returns(verificationDetailsDto);

            var expectedIpAddress = newIpAddress.Item;
            var verificationDetailsPatch = new VerificationDetailsPatch { IpAddress = expectedIpAddress.Some() };

            // Arrange
            await _profileService.UpdateAsync(userId, verificationDetailsPatch, CreateInitiationDto());

            // Assert
            await _verificationDetailsRepository.Received(1).SaveAsync(verificationDetailsDto);
            await _eventPublisher.Received(1).PublishAsync(
                Arg.Is<VerificationDetailsUpdated>(updated => updated.Changes.ContainsOnly("VerificationDetails.IpAddress")));

            verificationDetailsDto.IpAddress.Should().Be(expectedIpAddress);
            verificationDetailsDto.TaxResidence.Should().BeEquivalentTo(verificationDetailsDto.TaxResidence);
            verificationDetailsDto.Tin.Should().Be(verificationDetailsDto.Tin);
            verificationDetailsDto.IdDocumentNumber.Should().Be(verificationDetailsDto.IdDocumentNumber);
        }

        [Theory]
        public async Task ShouldUpdateVerificationDetails_WhenPatchHasTaxResidenceChanged(
            VerificationDetailsDto verificationDetailsDto,
            NonEmptyString taxResidence)
        {
            // Arrange
            var expected = verificationDetailsDto.TaxResidence == null
                ? new[] { taxResidence.Item }
                : verificationDetailsDto.TaxResidence.Append(taxResidence.Item).ToArray();

            var userId = verificationDetailsDto.UserId;
            _verificationDetailsRepository.FindAsync(userId)
                                          .Returns(verificationDetailsDto);

            var verificationDetailsPatch = new VerificationDetailsPatch { TaxResidence = expected.Some() };

            // Arrange
            await _profileService.UpdateAsync(userId, verificationDetailsPatch, CreateInitiationDto());

            // Assert
            await _verificationDetailsRepository.Received(1).SaveAsync(verificationDetailsDto);
            await _eventPublisher.Received(1).PublishAsync(
                Arg.Is<VerificationDetailsUpdated>(updated => updated.Changes.ContainsOnly("VerificationDetails.TaxResidence")));

            verificationDetailsDto.TaxResidence.Should().BeEquivalentTo(expected);
            verificationDetailsDto.IpAddress.Should().Be(verificationDetailsDto.IpAddress);
            verificationDetailsDto.Tin.Should().BeEquivalentTo(verificationDetailsDto.Tin);
            verificationDetailsDto.IdDocumentNumber.Should().Be(verificationDetailsDto.IdDocumentNumber);
        }

        [Theory]
        public async Task ShouldUpdateVerificationDetails_WhenPatchHasTinHasChanged(VerificationDetailsDto verificationDetailsDto, TinDto tin)
        {
            // Arrange
            var expected = tin;
            var userId = verificationDetailsDto.UserId;
            _verificationDetailsRepository.FindAsync(userId)
                                          .Returns(verificationDetailsDto);

            var verificationDetailsPatch = new VerificationDetailsPatch { Tin = expected.Some() };

            // Arrange
            await _profileService.UpdateAsync(userId, verificationDetailsPatch, CreateInitiationDto());

            // Assert
            await _verificationDetailsRepository.Received(1).SaveAsync(verificationDetailsDto);
            await _eventPublisher.Received(1).PublishAsync(
                Arg.Is<VerificationDetailsUpdated>(updated => updated.Changes.ContainsOnly("VerificationDetails.Tin")));
            verificationDetailsDto.Tin.Should().BeEquivalentTo(expected);
            verificationDetailsDto.TaxResidence.Should().BeEquivalentTo(verificationDetailsDto.TaxResidence);
            verificationDetailsDto.IpAddress.Should().Be(verificationDetailsDto.IpAddress);
            verificationDetailsDto.IdDocumentNumber.Should().Be(verificationDetailsDto.IdDocumentNumber);
        }

        [Theory]
        public async Task ShouldDetectSeveralChanges_WhenPatchHasSeveralChanges(VerificationDetailsDto verificationDetailsDto, 
                                                                                TinDto expectedTin, 
                                                                                NonEmptyString expectedIpAddress, 
                                                                                NonEmptyString newTaxResidence)
        {
            // Arrange
            var expectedChanges = new[]
            {
                "VerificationDetails.TaxResidence",
                "VerificationDetails.Tin",
                "VerificationDetails.IpAddress"
            };

            var expectedTaxResidence = verificationDetailsDto.TaxResidence == null
                ? new[] { newTaxResidence.Item }
                : verificationDetailsDto.TaxResidence.Append(newTaxResidence.Item).ToArray();

            var userId = verificationDetailsDto.UserId;
            _verificationDetailsRepository.FindAsync(userId)
                                          .Returns(verificationDetailsDto);

            var verificationDetailsPatch = new VerificationDetailsPatch
            {
                Tin = expectedTin.Some(),
                TaxResidence = expectedTaxResidence.Some(),
                IpAddress = expectedIpAddress.Item.Some()
            };

            // Arrange
            await _profileService.UpdateAsync(userId, verificationDetailsPatch, CreateInitiationDto());

            // Assert
            await _verificationDetailsRepository.Received(1).SaveAsync(verificationDetailsDto);
            await _eventPublisher.Received(1).PublishAsync(Arg.Is<VerificationDetailsUpdated>(updated => updated.Changes.ContainsOnly(expectedChanges)));

            verificationDetailsDto.Tin.Should().BeEquivalentTo(expectedTin);
            verificationDetailsDto.TaxResidence.Should().BeEquivalentTo(expectedTaxResidence);
            verificationDetailsDto.IpAddress.Should().Be(expectedIpAddress.Item);
        }

        [Theory(MaxTest = 5)]
        public async Task ShouldNotUpdateVerificationDetails_WhenPatchHasChanges_ButSimilarToExisting(
            VerificationDetailsDto verificationDetailsDto)
        {
            // Arrange
            var userId = verificationDetailsDto.UserId;
            _verificationDetailsRepository.FindAsync(userId)
                                          .Returns(verificationDetailsDto);

            var verificationDetailsPatch = new VerificationDetailsPatch
            {
                IpAddress = verificationDetailsDto.IpAddress.Some(),
                IdDocumentNumber = verificationDetailsDto.IdDocumentNumber.Some(),
                TaxResidence = ((string[])verificationDetailsDto.TaxResidence?.Clone()).Some()
            };

            // Arrange
            await _profileService.UpdateAsync(userId, verificationDetailsPatch, CreateInitiationDto());

            // Assert
            await _verificationDetailsRepository.DidNotReceiveWithAnyArgs().SaveAsync(verificationDetailsDto);
            await _eventPublisher.DidNotReceiveWithAnyArgs().PublishAsync();
        }

        [Theory]
        public async Task ShouldUpdatePersonalDetails_WhenHasChanged(PersonalDetailsDto oldDetails,
                                                                     PersonalDetailsDto newDetails)
        {
            var expectedChanges = new[]
            {
                "PersonalDetails.Email",
                "PersonalDetails.FirstName",
                "PersonalDetails.LastName",
                "PersonalDetails.FullName",
                "PersonalDetails.Nationality",
                "PersonalDetails.Birthdate",
                "PersonalDetails.ResidenceAddress"
            };

            // Arrange
            var userId = oldDetails.UserId;
            newDetails.UserId = userId;
            _personalDetailsRepository.FindAsync(userId).Returns(oldDetails);

            // Act
            var patch = MapToPatch(newDetails);
            await _profileService.UpdateAsync(userId, patch, CreateInitiationDto());

            // Assert
            await _personalDetailsRepository.Received(1).SaveAsync(oldDetails);
            await _eventPublisher.Received(1).PublishAsync(Arg.Is<PersonalDetailsUpdated>(updated => updated.Changes.ContainsOnly(expectedChanges)));

            oldDetails.Email.Should().Be(newDetails.Email);
            oldDetails.FirstName.Should().Be(newDetails.FirstName);
            oldDetails.LastName.Should().Be(newDetails.LastName);
            oldDetails.Nationality.Should().Be(newDetails.Nationality);
            oldDetails.ResidenceAddress.Should().Be(newDetails.ResidenceAddress);
            oldDetails.DateOfBirth.Should().Be(newDetails.DateOfBirth);
        }

        [Theory]
        public async Task ShouldUpdatePersonalDetails_WhenHasChangedExceptEmail(PersonalDetailsDto oldDetails,
                                                                                PersonalDetailsDto newDetails)
        {
            var expectedChanges = new[]
            {

                "PersonalDetails.FullName",
                "PersonalDetails.FirstName",
                "PersonalDetails.LastName",
                "PersonalDetails.Nationality",
                "PersonalDetails.Birthdate",
                "PersonalDetails.ResidenceAddress"
            };

            // Arrange
            var userId = oldDetails.UserId;
            newDetails.UserId = userId;
            newDetails.Email = oldDetails.Email;
            _personalDetailsRepository.FindAsync(userId).Returns(oldDetails);

            // Arrange
            var patch = MapToPatch(newDetails);
            await _profileService.UpdateAsync(userId, patch, CreateInitiationDto());

            // Assert
            await _personalDetailsRepository.Received(1).SaveAsync(oldDetails);
            await _eventPublisher.Received(1).PublishAsync(Arg.Is<PersonalDetailsUpdated>(updated => updated.Changes.ContainsOnly(expectedChanges)));

            oldDetails.Email.Should().Be(newDetails.Email);
            oldDetails.FirstName.Should().Be(newDetails.FirstName);
            oldDetails.LastName.Should().Be(newDetails.LastName);
            oldDetails.Nationality.Should().Be(newDetails.Nationality);
            oldDetails.ResidenceAddress.Should().Be(newDetails.ResidenceAddress);
            oldDetails.DateOfBirth.Should().Be(newDetails.DateOfBirth);
        }


        private static PersonalDetailsPatch MapToPatch(PersonalDetailsDto personalDetails)
        {
            return new PersonalDetailsPatch
            {
                FirstName = personalDetails.FirstName.SomeNotNull(),
                LastName = personalDetails.LastName.SomeNotNull(),
                DateOfBirth = personalDetails.DateOfBirth.SomeNotNull(),
                ResidenceAddress = personalDetails.ResidenceAddress.SomeNotNull(),
                Nationality = personalDetails.Nationality.SomeNotNull(),
                Email = personalDetails.Email.SomeNotNull()
            };
        }

        private static InitiationDto CreateInitiationDto([CallerMemberName] string testName = null) =>
            InitiationDto.Create("Test", testName);
    }
}