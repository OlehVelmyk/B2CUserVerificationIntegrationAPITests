using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface IUserNotificationService
    {
        Task SendAsync(UserResourcesChangedNotificationDto notification);

        Task SendAsync(TextNotificationDto notification);
    }
}