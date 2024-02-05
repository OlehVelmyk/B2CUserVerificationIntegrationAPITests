using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;

namespace WX.B2C.User.Verification.Core.Contracts.Repositories
{
    public interface IPersonalDetailsRepository
    {
        Task SaveAsync(PersonalDetailsDto dto);

        Task<PersonalDetailsDto> FindAsync(Guid userId);
    }
}