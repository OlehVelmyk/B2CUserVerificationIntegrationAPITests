using System;
using System.Security;
using WX.Core.TypeExtensions;

namespace WX.B2C.User.Verification.Facade.Controllers.Webhook.Services
{
    public interface ISecretValidator
    {
        bool Validate(string secret);
    }

    internal class SecretValidator : ISecretValidator
    {
        private readonly SecureString _secret;

        public SecretValidator(SecureString secret)
        {
            _secret = secret ?? throw new ArgumentNullException(nameof(secret));
        }

        public bool Validate(string secret)
        {
            if (string.IsNullOrEmpty(secret))
                return false;

            return string.Equals(secret, _secret.UnSecure());
        }
    }
}
