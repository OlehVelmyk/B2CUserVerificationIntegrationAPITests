using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Provider.Contracts.Models;
using WX.B2C.User.Verification.Provider.Services.System;

namespace WX.B2C.User.Verification.Unit.Tests.CheckProviders
{
    public class DuplicateScreeningCheckProviderTests
    {
        private readonly IDuplicateSearchService _duplicateSearchService;

        public DuplicateScreeningCheckProviderTests()
        {
            _duplicateSearchService = Substitute.For<IDuplicateSearchService>();
        }

        /// <summary>
        /// TODO finish test
        /// </summary>
        public void ShouldValidate()
        {
            var fullName = new FullNameDto {FirstName = "Test", LastName = "User" };
            var birthDate = DateTime.Now;
            var idDocumentNumber = new IdDocumentNumberDto { Type = IdentityDocumentType.Passport, Number = "12345" };

            var dictionaries = new Dictionary<string, object>
            {
                { XPathes.FullName, fullName },
                { XPathes.Birthdate, birthDate },
                { XPathes.IdDocumentNumber, idDocumentNumber }
            };
            var checkInputData = new CheckInputData(Guid.NewGuid(), null, dictionaries);

            var configuration = new DuplicateScreeningCheckConfiguration { };
            var validator = new DuplicateScreeningCheckDataValidator(configuration);

            var missingData = validator.Validate(checkInputData);

            missingData.Should().NotBeNull();
        }

        public async Task Should()
        {
            var fullName = new FullNameDto { FirstName = "Test", LastName = "User" };
            var birthDate = DateTime.Now;

            // Given
            _duplicateSearchService.FindAsync(Arg.Any<DuplicateSearchContext>())
                                   .Returns(DuplicateSearchResult.Empty);

            // Arrange
            var checkProvider = Get();
            var checkInputData = new DuplicateScreeningInputData{FullName = fullName, BirthDate = birthDate};

            // Act & Assert
            var runningResult = await checkProvider.RunAsync(checkInputData);
            runningResult.Should().NotBeNull();
            runningResult.Should().BeOfType<SyncCheckRunningResult>();

            var processingContext = new CheckProcessingContext(new CheckExternalDataDto());
            var checkResult = await checkProvider.GetResultAsync(processingContext);
            checkResult.Should().NotBeNull();
            checkResult.IsPassed.Should().BeTrue();
        }

        private DuplicateScreeningCheckRunner Get()
        {
            var configuration = new DuplicateScreeningCheckConfiguration { };
            return new DuplicateScreeningCheckRunner(_duplicateSearchService);
        }
    }
}
