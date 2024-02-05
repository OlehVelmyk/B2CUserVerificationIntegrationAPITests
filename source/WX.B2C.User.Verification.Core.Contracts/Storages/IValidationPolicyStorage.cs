using System.Collections.Generic;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Storages
{
    public interface IValidationPolicyStorage
    {
        Task<Dictionary<ActionType, ValidationRuleDto>> GetAsync(ValidationPolicySelectionContext selectionContext);
    }
}