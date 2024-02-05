using System.Collections.Generic;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos.UserEmails
{
    public class SendEmailParameters
    {
        public string TemplateId { get; set; }

        public string[] Emails { get; set; }

        public Dictionary<string, string> TemplateParameters { get; set; }

        public string FromName { get; set; }

        public string FromEmail { get; set; }
    }
}