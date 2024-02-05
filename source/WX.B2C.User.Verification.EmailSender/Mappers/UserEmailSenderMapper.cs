using System.Collections.Generic;
using WX.EmailSender.Commands.Parameters;

namespace WX.B2C.User.Verification.EmailSender.Mappers
{
    public interface IUserEmailSenderMapper
    {
        SendEmailParameters Map(Core.Contracts.Dtos.UserEmails.SendEmailParameters sendEmailParameters);
    }

    internal class UserEmailSenderMapper : IUserEmailSenderMapper
    {
        public SendEmailParameters Map(Core.Contracts.Dtos.UserEmails.SendEmailParameters sendEmailParameters)
        {
            return new SendEmailParameters
            {
                TemplateId = sendEmailParameters.TemplateId,
                TemplateParameters = sendEmailParameters.TemplateParameters ?? new Dictionary<string, string>(),
                Emails = sendEmailParameters.Emails,
                From = sendEmailParameters.FromEmail,
                FromName = sendEmailParameters.FromName,
            };
        }
    }
}