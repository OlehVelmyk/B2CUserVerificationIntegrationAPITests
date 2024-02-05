using System;
using WX.B2C.User.Verification.Facade.Controllers.Public.Dtos;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Mappers
{
    public interface IActionMapper
    {
        UserActionDto Map(Core.Contracts.Dtos.UserActionDto actionDto);
    }

    internal class ActionMapper : IActionMapper
    {
        public UserActionDto Map(Core.Contracts.Dtos.UserActionDto actionDto)
        {
            if (actionDto == null)
                throw new ArgumentNullException(nameof(actionDto));

            return new UserActionDto
            {
                Priority = actionDto.Priority,
                ActionType = actionDto.ActionType,
                Reason = actionDto.Reason,
                ActionData = actionDto.ActionData,
                IsOptional = actionDto.IsOptional
            };
        }
    }
}