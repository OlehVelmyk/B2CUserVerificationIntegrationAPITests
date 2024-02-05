using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface IUserEmailProvider
    {
        Task SendEmailAsync(Dtos.UserEmails.SendEmailParameters sendEmailParameters);
    }
}