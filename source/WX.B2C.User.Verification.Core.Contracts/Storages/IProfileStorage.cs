using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;

namespace WX.B2C.User.Verification.Core.Contracts.Storages
{
    public interface IProfileStorage
    {
        Task<VerificationDetailsDto> FindVerificationDetailsAsync(Guid userId);

        Task<PersonalDetailsDto> FindPersonalDetailsAsync(Guid userId);

        Task<PersonalDetailsDto> GetPersonalDetailsAsync(Guid userId);

        Task<PersonalDetailsDto> GetPersonalDetailsByExternalProfileIdAsync(string externalProfileId);

        Task<AddressDto> FindResidenceAddressAsync(Guid userId);

        Task<AddressDto> GetResidenceAddressAsync(Guid userId);

        Task<VerificationDetailsDto> GetVerificationDetailsAsync(Guid userId);

        Task<string> GetResidenceCountryAsync(Guid userId);

        Task<VerificationDetailsDto[]> GetVerificationDetailsAsync(Guid[] userId);
    }
}