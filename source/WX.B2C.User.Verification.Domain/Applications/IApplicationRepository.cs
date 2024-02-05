using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Domain
{
    public interface IApplicationRepository
    {
        Task<Application> FindAsync(Guid userId, ProductType productType);

        Task<Application> GetAsync(Guid id);

        Task SaveAsync(Models.Application application);
    }
}
