using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Dtos.UserEmails;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Core.Services.UserEmails;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.EmailSender;
using WX.B2C.User.Verification.EmailSender.Mappers;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.EmailSender.Commands;

namespace WX.B2C.User.Verification.Unit.Tests.Services
{
    internal class UserEmailServiceTests
    {
        private IProfileStorage _profileStorage;
        private IUserEmailService _userEmailService;
        private IApplicationStorage _applicationStorage;
        private IEmailSenderClient _emailSenderClient;

        [SetUp]
        public void Setup()
        {
            _emailSenderClient = Substitute.For<IEmailSenderClient>();
            _profileStorage = Substitute.For<IProfileStorage>();
            _applicationStorage = Substitute.For<IApplicationStorage>();

            var userEmailProvider = new UserEmailProvider(_emailSenderClient, new UserEmailSenderMapper());
            var collectionStepStorage = Substitute.For<ICollectionStepStorage>();
            var xpathParser = Substitute.For<IXPathParser>();
            var hostSettingsProvider = Substitute.For<IHostSettingsProvider>();
            var parametersProvider = new ParametersProvider();
            _userEmailService = new UserEmailService(
                userEmailProvider,
                _profileStorage,
                collectionStepStorage,
                xpathParser,
                hostSettingsProvider,
                _applicationStorage,
                parametersProvider);
        }

        [Theory]
        public async Task ShouldSetClearTemplateId(Guid userId, string email)
        {
            // Arrange
            _profileStorage.GetPersonalDetailsAsync(userId).Returns(
                new PersonalDetailsDto
                {
                    Email = email
                });
            _applicationStorage.IsAutomatedAsync(userId, ProductType.WirexBasic).Returns(true);

            // Act
            await _userEmailService.SendAsync(new ApplicationStateChangedEmailContext
                { NewState = ApplicationState.Approved, UserId = userId });

            // Assert
            await _emailSenderClient.Received(1).SendEmail(
                Arg.Is<WX.EmailSender.Commands.Parameters.SendEmailParameters>(p => p.TemplateId.Equals(EmailSendTemplates.UserVerificationApproved)),
                CancellationToken.None);
        }
    }
}