using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Storages
{
    public interface ITaskStorage
    {
        Task<TaskDto[]> GetAllAsync(Guid userId, TaskType? taskType = null);

        Task<TaskDto[]> FindByCheckIdAsync(Guid checkId);

        Task<TaskDto[]> FindByStepIdAsync(Guid collectionStepId);

        Task<VerificationTask[]> FindAsync(Guid userId, IEnumerable<Guid> variantIds);

        Task<TaskDto> GetAsync(Guid taskId);

        Task<TaskDto> FindAsync(Guid taskId, Guid userId);

        Task<TaskDto[]> GetByApplicationIdAsync(Guid applicationId);

        Task<TaskDto> FindAsync(Guid applicationId, TaskType taskType);

        Task<TaskDto> GetAsync(Guid applicationId, TaskType taskType);

        Task<TaskPriorityGroupDto[]> GetPriorityGroupsAsync(Guid userId);
    }
}