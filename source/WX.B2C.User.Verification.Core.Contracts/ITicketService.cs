using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Ticket;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface ITicketService
    {
         Task SendAsync(CheckTicketContext context);

         Task SendAsync(ApplicationTicketContext ticketContext);

         Task SendAsync(TicketContext ticketContext);
         
         Task SendAsync(ReviewCollectionStepTicketContext ticketContext);

         Task SendAsync(AccountAlertTicketContext ticketContext);

        Task SendAsync(ExceededCheckResubmitAttemptsTicketContext ticketContext);
    }
}