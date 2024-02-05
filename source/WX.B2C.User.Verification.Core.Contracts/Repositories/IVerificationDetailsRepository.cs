using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;

namespace WX.B2C.User.Verification.Core.Contracts.Repositories
{
    public interface IVerificationDetailsRepository
    {
        Task<VerificationDetailsDto> FindAsync(Guid userId);

        Task SaveAsync(VerificationDetailsDto verificationDetailsDto);
    }
}