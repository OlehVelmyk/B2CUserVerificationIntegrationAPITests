using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface IProfileService
    {
        Task UpdateAsync(Guid userId, PersonalDetailsPatch patch, InitiationDto initiationDto);

        Task UpdateAsync(Guid userId, VerificationDetailsPatch patch, InitiationDto initiationDto);

        Task UpdateAsync(Guid userId, ExternalProfileDto externalProfileDto);
    }
}