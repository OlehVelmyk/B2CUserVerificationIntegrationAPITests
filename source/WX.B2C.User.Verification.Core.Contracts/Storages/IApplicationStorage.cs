using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Storages
{
    public interface IApplicationStorage
    {
        Task<ApplicationState> GetStateAsync(Guid userId, ProductType productType);

        Task<ApplicationState?> FindStateAsync(Guid userId, ProductType productType);

        Task<ApplicationDto> GetAsync(Guid applicationId);

        Task<ApplicationDto> FindAsync(Guid userId, ProductType productType);

        Task<Guid?> FindIdAsync(Guid userId, ProductType productType);

        Task<ApplicationDto> FindAsync(Guid userId, Guid applicationId);

        Task<ApplicationDto[]> FindAsync(Guid userId);

        Task<ApplicationDto[]> FindAsync(Guid userId, ApplicationState applicationState);

        Task<ApplicationDto[]> FindAssociatedWithTaskAsync(Guid taskId);

        Task<bool> IsAutomatedAsync(Guid userId, ProductType productType);
    }
}
