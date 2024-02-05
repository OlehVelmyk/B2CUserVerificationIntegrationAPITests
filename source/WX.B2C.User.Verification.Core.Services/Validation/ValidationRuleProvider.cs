using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Core.Contracts.Providers;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Core.Contracts.Validation;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Core.Services.Validation
{
    public class ValidationRuleProvider : IValidationRuleProvider
    {
        private readonly IPolicySelectionContextProvider _selectionContextProvider;
        private readonly IValidationPolicyStorage _validationPolicyStorage;
        private readonly IEnumerable<IValidationRuleFilter> _filters;

        public ValidationRuleProvider(
            IPolicySelectionContextProvider selectionContextProvider,
            IValidationPolicyStorage validationPolicyStorage,
            IEnumerable<IValidationRuleFilter> filters)
        {
            _validationPolicyStorage = validationPolicyStorage ?? throw new ArgumentNullException(nameof(validationPolicyStorage));
            _selectionContextProvider = selectionContextProvider ?? throw new ArgumentNullException(nameof(selectionContextProvider));
            _filters = filters ?? throw new ArgumentNullException(nameof(filters));
        }

        public async Task<Dictionary<ActionType, ValidationRuleDto>> GetAsync(Guid userId, params ActionType[] actionTypes)
        {
            var selectionContext = await _selectionContextProvider.GetValidationContextAsync(userId);
            var validationRules = await _validationPolicyStorage.GetAsync(selectionContext);

            if (!actionTypes.IsEmpty())
            {
                validationRules = validationRules
                                  .Where(pair => pair.Key.In(actionTypes))
                                  .ToDictionary(pair => pair.Key, pair => pair.Value);
            }

            var result = new Dictionary<ActionType, ValidationRuleDto>();
            foreach (var (actionType, validationRule) in validationRules)
            {
                var filterContext = new ValidationRuleFilterContext
                {
                    UserId = userId,
                    Country = selectionContext.Country,
                    ActionType = actionType
                };
                result.Add(actionType, await ApplyFilters(validationRule, filterContext));
            }
            return result;
        }

        public async Task<ValidationRuleDto> FindAsync(Guid userId, ActionType actionType)
        {
            var rules = await GetAsync(userId, actionType);
            return rules.GetValueOrDefault(actionType);
        }

        private async Task<ValidationRuleDto> ApplyFilters(ValidationRuleDto validationRule, ValidationRuleFilterContext context)
        {
            foreach (var filter in _filters)
            {
                if (filter.CanApply(context))
                    validationRule = await filter.ApplyAsync(validationRule, context);
            }

            return validationRule;
        }
    }
}