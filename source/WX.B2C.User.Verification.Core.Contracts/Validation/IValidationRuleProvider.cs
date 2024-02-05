using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Core.Contracts.Validation
{
    public interface IValidationRuleProvider
    {
        Task<Dictionary<ActionType, ValidationRuleDto>> GetAsync(Guid userId, params ActionType[] actionTypes);

        Task<ValidationRuleDto> FindAsync(Guid userId, ActionType actionType);
    }
}