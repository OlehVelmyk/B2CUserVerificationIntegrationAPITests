using System.Security;
using WX.EmailSender.Commands.Configuration;

namespace WX.B2C.User.Verification.EmailSender
{
    internal class EmailSenderOptions : IEmailSenderOptions
    {
        public EmailSenderOptions(SecureString emailSenderStorageConnectionString)
        {
            StorageConnectionString = emailSenderStorageConnectionString;
        }

        public SecureString StorageConnectionString { get; }
    }
}