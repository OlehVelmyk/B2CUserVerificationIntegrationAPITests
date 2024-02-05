using System;
using System.Threading;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.EmailSender.Mappers;
using WX.EmailSender.Commands;

namespace WX.B2C.User.Verification.EmailSender
{
    public class UserEmailProvider : IUserEmailProvider
    {
        private readonly IEmailSenderClient _emailSenderClient;
        private readonly IUserEmailSenderMapper _userEmailSenderMapper;

        public UserEmailProvider(
            IEmailSenderClient emailSenderClient,
            IUserEmailSenderMapper userEmailSenderMapper)
        {
            _emailSenderClient = emailSenderClient ?? throw new ArgumentNullException(nameof(emailSenderClient));
            _userEmailSenderMapper = userEmailSenderMapper ?? throw new ArgumentNullException(nameof(userEmailSenderMapper));
        }

        public async Task SendEmailAsync(Core.Contracts.Dtos.UserEmails.SendEmailParameters sendEmailParameters)
        {
            var sendEmail = _userEmailSenderMapper.Map(sendEmailParameters);

            await _emailSenderClient.SendEmail(sendEmail, CancellationToken.None);
        }
    }
}