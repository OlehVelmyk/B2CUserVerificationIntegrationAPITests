using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Core.Services;
using WX.B2C.User.Verification.Core.Services.RequiredData;
using WX.B2C.User.Verification.DataAccess;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries.TestData;
using WX.B2C.User.Verification.Unit.Tests.Extensions;

namespace WX.B2C.User.Verification.Unit.Tests.Services
{
    public class ProfileDataProviderTests
    {
        private IProfileStorage _profileStorage;
        private ICollectionStepStorage _collectionStepStorage;
        private IDocumentStorage _documentStorage;
        private ProfileProviderFactory _factory;
        private IXPathParser _xPathParser;

        [SetUp]
        public void Setup()
        {
            _profileStorage = Substitute.For<IProfileStorage>();
            _collectionStepStorage = Substitute.For<ICollectionStepStorage>();
            _documentStorage = Substitute.For<IDocumentStorage>();
            //TODO temporary use real implementation
            _xPathParser = new XPathParser(new HardcodedDocumentTypeProvider());
            _factory = new ProfileProviderFactory(_profileStorage, _collectionStepStorage, _documentStorage, _xPathParser);
        }

        [Theory(MaxTest = 10)]
        public async Task ReadAsync_ShouldFindDataInProfile(Guid userId,
                                                            XPath xPath,
                                                            NotEmptyPersonalDetails personalDetailsDto,
                                                            NotEmptyVerificationDetails verificationDetailsDto)
        {
            // Arrange
            var provider = _factory.Create(userId);
            _profileStorage.FindPersonalDetailsAsync(userId).Returns(personalDetailsDto);
            _profileStorage.FindVerificationDetailsAsync(userId).Returns(verificationDetailsDto);
            _documentStorage.FindSubmittedDocumentsAsync(userId).Returns(new Documents().All);

            // Act
            var data = await provider.ReadAsync(new string[] { xPath });

            // Assert
            data.HasValue(xPath).Should().BeTrue();
            data[xPath].Should().NotBeNull();
        }

        [Theory(MaxTest = 10)]
        public async Task ReadNotRequestedAsync_ShouldFindDataInProfile(Guid userId,
                                                                        XPath xPath,
                                                                        NotEmptyPersonalDetails personalDetailsDto,
                                                                        NotEmptyVerificationDetails verificationDetailsDto)
        {
            // Arrange
            var provider = _factory.Create(userId);
            _profileStorage.FindPersonalDetailsAsync(userId).Returns(personalDetailsDto);
            _profileStorage.FindVerificationDetailsAsync(userId).Returns(verificationDetailsDto);
            _documentStorage.FindSubmittedDocumentsAsync(userId).Returns(new Documents().All);

            // Act
            var data = await provider.ReadAsync(new string[] { xPath });

            // Assert
            data.HasValue(xPath).Should().BeTrue();
            data[xPath].Should().NotBeNull();
        }

        [Theory(MaxTest = 10)]
        public async Task ReadNotRequestedAsync_ShouldNotFindDataInProfile_WhenItIsRequested(Guid userId,
            XPath xPath,
            NotEmptyPersonalDetails personalDetailsDto,
            NotEmptyVerificationDetails verificationDetailsDto)
        {
            // Arrange
            var provider = _factory.Create(userId);
            _profileStorage.FindPersonalDetailsAsync(userId).Returns(personalDetailsDto);
            _profileStorage.FindVerificationDetailsAsync(userId).Returns(verificationDetailsDto);
            _documentStorage.FindSubmittedDocumentsAsync(userId).Returns(new Documents().All);
            _collectionStepStorage.FindRequestedAsync(userId).Returns(new[] { new CollectionStepDto { XPath = xPath } });

            // Act
            var data = await provider.ReadNotRequestedAsync(new string[] { xPath });

            // Assert
            data.HasValue(xPath).Should().BeFalse();
            await _documentStorage.DidNotReceiveWithAnyArgs().FindSubmittedDocumentsAsync(userId);
            await _profileStorage.DidNotReceiveWithAnyArgs().FindVerificationDetailsAsync(userId);
            await _profileStorage.DidNotReceiveWithAnyArgs().FindPersonalDetailsAsync(userId);
        }

        [Theory]
        public async Task ReadAsync_ShouldBeThreadSafe(Guid userId,
                                                       XPath xPath,
                                                       NotEmptyPersonalDetails personalDetailsDto,
                                                       NotEmptyVerificationDetails verificationDetailsDto)
        {
            // Arrange
            var provider = _factory.Create(userId);
            _profileStorage.FindPersonalDetailsAsync(userId).Returns(personalDetailsDto);
            _profileStorage.FindVerificationDetailsAsync(userId).Returns(verificationDetailsDto);
            _documentStorage.FindSubmittedDocumentsAsync(userId).Returns(new Documents().All);

            // Act
            var act = TaskRunner.RepeatInParallel(() => provider.ReadAsync(XPathArbitrary.Supported), 10);

            // Assert
            await act.Should().NotThrowAsync();

            await _documentStorage.Received(1).FindSubmittedDocumentsAsync(userId);
            await _profileStorage.Received(1).FindVerificationDetailsAsync(userId);
            await _profileStorage.Received(1).FindPersonalDetailsAsync(userId);
        }

        [Theory]
        public async Task ReadNotRequestedAsync_ShouldBeThreadSafe(Guid userId,
                                                                   XPath xPath,
                                                                   NotEmptyVerificationDetails verificationDetailsDto)
        {
            // Arrange
            var provider = _factory.Create(userId);
            _profileStorage.FindVerificationDetailsAsync(userId).Returns(verificationDetailsDto);
            _documentStorage.FindSubmittedDocumentsAsync(userId).Returns(new Documents().All);
            var requestedCollectionSteps = XPathArbitrary.Personal.Select(path => new CollectionStepDto { XPath = path }).ToArray();
            _collectionStepStorage.FindRequestedAsync(userId).Returns(requestedCollectionSteps);

            // Act
            var act = TaskRunner.RepeatInParallel(() => provider.ReadNotRequestedAsync(XPathArbitrary.Supported), 10);

            // Assert
            await act.Should().NotThrowAsync();

            await _documentStorage.Received(1).FindSubmittedDocumentsAsync(userId);
            await _profileStorage.Received(1).FindVerificationDetailsAsync(userId);
            await _collectionStepStorage.Received(1).FindRequestedAsync(userId);
            await _profileStorage.DidNotReceiveWithAnyArgs().FindPersonalDetailsAsync(userId);
        }

        [Theory]
        public async Task ReadAsync_ShouldReadDocumentsOnce(Guid userId)
        {
            // Arrange
            var provider = _factory.Create(userId);
            var xPathes = new Documents().All.Select(dto => (string)new DocumentXPath(dto.Category, dto.Type));

            // Act
            await provider.ReadAsync(xPathes);
            await provider.ReadAsync(xPathes);

            // Assert
            await _documentStorage.Received(1).FindSubmittedDocumentsAsync(userId);
            await _profileStorage.DidNotReceiveWithAnyArgs().FindPersonalDetailsAsync(userId);
            await _profileStorage.DidNotReceiveWithAnyArgs().FindVerificationDetailsAsync(userId);
        }

        [Theory]
        public async Task ReadAsync_ShouldReadPersonalDetailsOnce(Guid userId, PersonalDetailsDto personalDetailsDto)
        {
            // Arrange
            var provider = _factory.Create(userId);
            var xPathes = XPathArbitrary.Personal;

            // Act
            await provider.ReadAsync(xPathes);
            await provider.ReadAsync(xPathes);

            // Assert
            await _profileStorage.Received(1).FindPersonalDetailsAsync(userId);
            await _documentStorage.DidNotReceiveWithAnyArgs().FindSubmittedDocumentsAsync(userId);
            await _profileStorage.DidNotReceiveWithAnyArgs().FindVerificationDetailsAsync(userId);
        }

        [Theory]
        public async Task ReadAsync_ShouldReadVerificationDetailsOnce(Guid userId, VerificationDetailsDto personalDetailsDto)
        {
            // Arrange
            var provider = _factory.Create(userId);
            var xPathes = XPathArbitrary.Verification;

            // Act
            await provider.ReadAsync(xPathes);
            await provider.ReadAsync(xPathes);

            // Assert
            await _profileStorage.Received(1).FindVerificationDetailsAsync(userId);
            await _documentStorage.DidNotReceiveWithAnyArgs().FindSubmittedDocumentsAsync(userId);
            await _profileStorage.DidNotReceiveWithAnyArgs().FindPersonalDetailsAsync(userId);
        }

        [Theory]
        public async Task ReadAsync_ShouldReadCorrectDataFromProfile(Guid userId,
                                                                     NotEmptyPersonalDetails personalDetailsDto,
                                                                     NotEmptyVerificationDetails verificationDetailsDto)
        {
            // Arrange
            var provider = _factory.Create(userId);
            _profileStorage.FindPersonalDetailsAsync(userId).Returns(personalDetailsDto);
            _profileStorage.FindVerificationDetailsAsync(userId).Returns(verificationDetailsDto);
            var documents = new Documents();
            _documentStorage.FindSubmittedDocumentsAsync(userId).Returns(documents.All);

            // Act
            var data = await provider.ReadAsync(XPathArbitrary.Supported);

            // Assert
            data[XPathes.IpAddress].Should().Be(verificationDetailsDto.IpAddress);
            data[XPathes.TaxResidence].Should().BeEquivalentTo(verificationDetailsDto.TaxResidence);
            data[XPathes.IdDocumentNumber].Should().Be(verificationDetailsDto.IdDocumentNumber);
            data[XPathes.Tin].Should().Be(verificationDetailsDto.Tin);
            data[XPathes.FirstName].Should().Be(personalDetailsDto.FirstName);
            data[XPathes.LastName].Should().Be(personalDetailsDto.LastName);
            data[XPathes.Birthdate].Should().Be(personalDetailsDto.DateOfBirth);
            data[XPathes.ResidenceAddress].Should().Be(personalDetailsDto.ResidenceAddress);
            data[XPathes.PersonalNationality].Should().Be(personalDetailsDto.Nationality);
            data[XPathes.Email].Should().Be(personalDetailsDto.Email);
            data[XPathes.ProofOfIdentityDocument].Should().BeOfType<DocumentDto>().Subject.Should().BeEquivalentTo(documents.ProofOfIdentity);
            data[XPathes.ProofOfAddressDocument].Should().BeOfType<DocumentDto>().Subject.Should().BeEquivalentTo(documents.ProofOfAddress);
            data[XPathes.ProofOfFundsDocument].Should().BeOfType<DocumentDto>().Subject.Should().BeEquivalentTo(documents.ProofOfFunds);
            data[XPathes.SelfiePhoto].Should().BeOfType<DocumentDto>().Subject.Should().BeEquivalentTo(documents.SelfiePhoto);
            data[XPathes.SelfieVideo].Should().BeOfType<DocumentDto>().Subject.Should().BeEquivalentTo(documents.SelfieVideo);
            data[XPathes.W9Form].Should().BeOfType<DocumentDto>().Subject.Should().BeEquivalentTo(documents.W9Form);

            data[XPathes.FullName]
                .Should()
                .BeOfType<FullNameDto>()
                .Which.Should()
                .Match<FullNameDto>(name => name.FirstName == personalDetailsDto.FirstName && name.LastName == personalDetailsDto.LastName);
        }

        [Theory]
        public async Task ReadAsync_ShouldReadSubDataWhenRequested(Guid userId, 
                                                                   NotEmptyPersonalDetails personalDetailsDto, 
                                                                   NotEmptyVerificationDetails verificationDetailsDto)
        {
            // Arrange
            var provider = _factory.Create(userId);
            _profileStorage.FindPersonalDetailsAsync(userId).Returns(personalDetailsDto);
            _profileStorage.FindVerificationDetailsAsync(userId).Returns(verificationDetailsDto);
            var documents = new Documents();
            _documentStorage.FindSubmittedDocumentsAsync(userId).Returns(documents.All);

            // Act
            var data = await provider.ReadAsync(XPathArbitrary.SubData);

            // Assert
            data[XPathes.ResidenceAddress.Country].Should().Be(personalDetailsDto.ResidenceAddress.Country);
            data[XPathes.ResidenceAddress.Line1].Should().Be(personalDetailsDto.ResidenceAddress.Line1);
            data[XPathes.ResidenceAddress.ZipCode].Should().Be(personalDetailsDto.ResidenceAddress.ZipCode);
            data[XPathes.IdDocumentNumber.Number].Should().Be(verificationDetailsDto.IdDocumentNumber.Number);
            data[XPathes.IdDocumentNumber.Type].Should().Be(verificationDetailsDto.IdDocumentNumber.Type);
            data[XPathes.Tin.Number].Should().Be(verificationDetailsDto.Tin.Number);
            data[XPathes.Tin.Type].Should().Be(verificationDetailsDto.Tin.Type);
        }

        [Theory]
        public async Task ReadAsync_ShouldNotFindSubData_WhenParentIsNull(Guid userId, NotEmptyPersonalDetails personalDetailsDto, NotEmptyVerificationDetails verificationDetailsDto)
        {
            // Arrange
            var provider = _factory.Create(userId);
            personalDetailsDto.ResidenceAddress = null;
            verificationDetailsDto.Tin = null;
            verificationDetailsDto.IdDocumentNumber = null;

            _profileStorage.FindPersonalDetailsAsync(userId).Returns(personalDetailsDto);
            _profileStorage.FindVerificationDetailsAsync(userId).Returns(verificationDetailsDto);
            var documents = new Documents();
            _documentStorage.FindSubmittedDocumentsAsync(userId).Returns(documents.All);

            // Act
            var data = await provider.ReadAsync(XPathArbitrary.SubData);

            // Assert
            data.ValueOrNull(XPathes.ResidenceAddress.Country).Should().BeNull();
            data.ValueOrNull(XPathes.ResidenceAddress.Line1).Should().BeNull();
            data.ValueOrNull(XPathes.ResidenceAddress.ZipCode).Should().BeNull();
            data.ValueOrNull(XPathes.IdDocumentNumber.Number).Should().BeNull();
            data.ValueOrNull(XPathes.IdDocumentNumber.Type).Should().BeNull();
            data.ValueOrNull(XPathes.Tin.Number).Should().BeNull();
            data.ValueOrNull(XPathes.Tin.Type).Should().BeNull();
        }


        [Theory]
        public async Task ShouldInputDataBeSerializable(Guid userId, NotEmptyPersonalDetails personalDetailsDto,
                                                                     NotEmptyVerificationDetails verificationDetailsDto)
        {
            // Arrange
            var provider = _factory.Create(userId);
            _profileStorage.FindPersonalDetailsAsync(userId).Returns(personalDetailsDto);
            _profileStorage.FindVerificationDetailsAsync(userId).Returns(verificationDetailsDto);
            var documents = new Documents();
            _documentStorage.FindSubmittedDocumentsAsync(userId).Returns(documents.All);

            // Act
            var data = await provider.ReadAsync(XPathArbitrary.Supported);

            // Assert
            var checkInputData = new CheckInputDataDto(data);

            var contractJsonSerializer = new DataContractJsonSerializer(typeof(CheckInputDataDto));
            contractJsonSerializer.WriteObject(new MemoryStream(), checkInputData);
        }

    }
}