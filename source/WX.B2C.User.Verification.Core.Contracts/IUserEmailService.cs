using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos.UserEmails;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface IUserEmailService
    {
        public Task SendAsync(ApplicationStateChangedEmailContext context);

        public Task SendAsync(CollectionStepRequestedEmailContext context);

        public Task SendAsync(ReminderSentEmailContext context);
    }
}