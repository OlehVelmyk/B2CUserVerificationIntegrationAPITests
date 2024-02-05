using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Exceptions;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.LexisNexis
{
    public class BridgerCredentialsService : IBridgerCredentialsService
    {
        private readonly IBridgerApiClientFactory _bridgerApiClientFactory;
        private readonly IBridgerCredentialsRepository _bridgerCredentialsRepository;
        private readonly IBridgerCredentialsProvider _bridgerCredentialsProvider;
        private readonly IEncryptProvider _encryptProvider;

        public BridgerCredentialsService(IBridgerApiClientFactory bridgerApiClientFactory,
                                         IBridgerCredentialsRepository bridgerCredentialsRepository,
                                         IBridgerCredentialsProvider bridgerCredentialsProvider,
                                         IEncryptProvider encryptProvider)
        {
            _bridgerApiClientFactory = bridgerApiClientFactory ?? throw new ArgumentNullException(nameof(bridgerApiClientFactory));
            _bridgerCredentialsRepository = bridgerCredentialsRepository ?? throw new ArgumentNullException(nameof(bridgerCredentialsRepository));
            _bridgerCredentialsProvider = bridgerCredentialsProvider ?? throw new ArgumentNullException(nameof(bridgerCredentialsProvider));
            _encryptProvider = encryptProvider ?? throw new ArgumentNullException(nameof(encryptProvider));
        }

        public async Task<int> GetDaysUntilPasswordExpiresAsync(string userId)
        {
            var password = await _bridgerCredentialsProvider.GetPasswordAsync(userId);

            var client = await _bridgerApiClientFactory.CreateAsync();
            return await client.GetDaysUntilPasswordExpiresAsync(userId, password);
        }

        public async Task UpdateAsync(string userId, string newPassword, bool propagate = true)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentNullException(nameof(newPassword));

            if (propagate)
                await PropagateAsync(userId, newPassword);

            var encryptedPassword = _encryptProvider.EncryptText(newPassword);
            var credentials = new BridgerCredentialsDto
            {
                UserId = userId,
                EncryptedPassword = encryptedPassword
            };
            await _bridgerCredentialsRepository.SaveAsync(credentials);
        }

        private async Task PropagateAsync(string userId, string newPassword)
        {
            var currentPassword = await _bridgerCredentialsProvider.GetPasswordAsync(userId);
            var client = await _bridgerApiClientFactory.CreateAsync();
            var success = await client.ChangeAccountPasswordAsync(userId, currentPassword, newPassword);
            if (!success) throw new BridgerPasswordUpdateException(userId);
        }
    }
}