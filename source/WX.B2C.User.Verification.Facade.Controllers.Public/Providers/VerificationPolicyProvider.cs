using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Providers
{
    public interface IVerificationPolicyProvider
    {
        Task<Guid> GetAsync(VerificationPolicySelectionContext selectionContext);
    }

    internal class VerificationPolicyProvider : IVerificationPolicyProvider
    {
        private readonly IVerificationPolicyStorage _verificationPolicyStorage;

        public VerificationPolicyProvider(IVerificationPolicyStorage verificationPolicyStorage)
        {
            _verificationPolicyStorage = verificationPolicyStorage ?? throw new ArgumentNullException(nameof(verificationPolicyStorage));
        }

        public Task<Guid> GetAsync(VerificationPolicySelectionContext selectionContext) =>
            _verificationPolicyStorage.GetIdAsync(selectionContext);
    }
}
