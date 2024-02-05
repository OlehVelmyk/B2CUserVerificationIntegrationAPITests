using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Infrastructure.Common
{
    internal class BridgerCredentialsProvider : IBridgerCredentialsProvider
    {
        private readonly IEncryptProvider _encryptProvider;
        private readonly IBridgerCredentialsStorage _bridgerCredentialsStorage;

        public BridgerCredentialsProvider(IEncryptProvider encryptProvider,
                                          IBridgerCredentialsStorage bridgerCredentialsStorage)
        {
            _encryptProvider = encryptProvider ?? throw new ArgumentNullException(nameof(encryptProvider));
            _bridgerCredentialsStorage = bridgerCredentialsStorage ?? throw new ArgumentNullException(nameof(bridgerCredentialsStorage));
        }

        public async Task<string> GetPasswordAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            var encryptedPassword = await _bridgerCredentialsStorage.GetPasswordAsync(userId);
            return _encryptProvider.DecryptText(encryptedPassword);
        }
    }
}
