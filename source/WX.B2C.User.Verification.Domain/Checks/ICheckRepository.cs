using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Domain
{
    public interface ICheckRepository
    {
        Task<Check> GetAsync(Guid checkId);

        Task SaveAsync(Check check);
    }
}
