using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Services
{
    internal class UserActionValidatorService : IUserActionValidatorService
    {
        private readonly IActionService _actionService;
        private readonly IActionTypeMapper _actionTypeMapper;

        public UserActionValidatorService(IActionService actionService, IActionTypeMapper actionTypeMapper)
        {
            _actionService = actionService ?? throw new ArgumentNullException(nameof(actionService));
            _actionTypeMapper = actionTypeMapper ?? throw new ArgumentNullException(nameof(actionTypeMapper));
        }

        public Task<bool> ValidateAsync(Guid userId, DocumentCategory documentCategory, string documentType)
        {
            var actionType = _actionTypeMapper.Map(documentCategory, documentType);

            if (!actionType.HasValue)
                return Task.FromResult(false);

            return ValidateAsync(userId, actionType.Value);
        }

        public async Task<bool> ValidateAsync(Guid userId, ActionType actionType)
        {
            var availableUserActions = await _actionService.GetAsync(userId);
            return availableUserActions.Any(a => a.ActionType.Equals(actionType));
        }
    }
}
