using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Services
{
    public interface IUserActionValidatorService
    {
        Task<bool> ValidateAsync(Guid userId, ActionType actionType);

        Task<bool> ValidateAsync(Guid userId, DocumentCategory documentCategory, string documentType);
    }
}