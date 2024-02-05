using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Core.Contracts.Storages
{
    public interface IReminderStorage
    {
        Task<UserReminderDto[]> FindAsync(Guid userId);

        Task<int> CountAsync(Guid userId, Guid targetId);
    }
}
