using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Onfido.Client;
using WX.B2C.User.Verification.Onfido.Configurations;
using WX.B2C.User.Verification.Onfido.Models;
using WX.B2C.User.Verification.Onfido.Processors;
using WX.B2C.User.Verification.Onfido.Runners;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Unit.Tests.CheckProviders
{
    public class IdentityDocumentsCheckProviderTests
    {
        private readonly IIdentityDocumentCheckResultProcessor _checkResultValidator;
        private readonly IOnfidoApiClientFactory _clientFactory;
        private readonly ICountryDetailsProvider _countryDetailsProvider;

        public IdentityDocumentsCheckProviderTests()
        {
            _clientFactory = Substitute.For<IOnfidoApiClientFactory>();
            _countryDetailsProvider = Substitute.For<ICountryDetailsProvider>();
        }

        public void ShouldValidate()
        {
            var applicantId = "";
            var fullName = new FullNameDto {FirstName = "", LastName = "" };
            var birthDate = DateTime.Now;

            var checkInputData = new CheckInputData(Guid.NewGuid(), null, new Dictionary<string, object>
            {
                { XPathes.FullName, fullName },
                { XPathes.Birthdate, birthDate }
            });
        }

        public async Task Should()
        {
            // Given
            var applicantId = "";
            var fullName = new FullNameDto { FirstName = "", LastName = "" };
            var birthDate = DateTime.Now;

            // Arrange
            var configuration = new IdentityDocumentsCheckConfiguration();
            var checkInputData = new IdentityDocumentsCheckInputData
            {
                ApplicantId = applicantId,
                FullName = fullName,
                BirthDate = birthDate
            };

            // Act & Assert
            var checkRunner = new IdentityDocumentsCheckRunner(configuration, _clientFactory, _checkResultValidator);
            var runningResult = await checkRunner.RunAsync(checkInputData);
            runningResult.Should().NotBeNull();
            var asyncRunningResult = runningResult.Should().BeOfType<AsyncCheckRunningResult>().Subject;

            var processingContext = CheckProcessingContext.Create(asyncRunningResult.ExternalData);
            var checkResult = await checkRunner.GetResultAsync(processingContext);
            checkResult.IsPassed.Should().BeTrue();
        }
    }
}