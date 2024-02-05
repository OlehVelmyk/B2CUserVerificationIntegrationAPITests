using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Core.Contracts.Repositories
{
    public interface IApplicationStateChangelogRepository
    {
        Task<ApplicationStateChangelogDto> FindAsync(Guid applicationId);

        Task SaveAsync(ApplicationStateChangelogDto applicationStateChangelog);
    }
}
