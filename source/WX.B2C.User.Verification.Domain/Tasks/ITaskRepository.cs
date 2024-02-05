using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Domain
{
    public interface ITaskRepository
    {
        Task<VerificationTask> GetAsync(Guid id);

        Task SaveAsync(VerificationTask task);
    }
}
