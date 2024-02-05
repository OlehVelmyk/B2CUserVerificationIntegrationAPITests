using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Core.Contracts.Validation;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Facade.Controllers.Public.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Public.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Public.Mappers;
using WX.B2C.User.Verification.Facade.Controllers.Public.Responses;
using WX.B2C.User.Verification.Facade.Controllers.Public.Services;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware;
using static WX.B2C.User.Verification.Facade.Controllers.Public.Extensions.Errors;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/verification/validations")]
    public class ValidationController : ApiController
    {
        private readonly IValidationRuleProvider _validationPolicyProvider;
        private readonly IValidationRulesMapper _validationRulesMapper;
        private readonly IActionTypeMapper _actionTypeMapper;

        public ValidationController(
                                    IValidationRuleProvider validationPolicyProvider,
                                    IValidationRulesMapper validationRulesMapper,
                                    IActionTypeMapper actionTypeMapper)
        {
            _validationPolicyProvider = validationPolicyProvider ?? throw new ArgumentNullException(nameof(validationPolicyProvider));
            _validationRulesMapper = validationRulesMapper ?? throw new ArgumentNullException(nameof(validationRulesMapper));
            _actionTypeMapper = actionTypeMapper ?? throw new ArgumentNullException(nameof(actionTypeMapper));
        }

        [HttpGet]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ValidationRulesDto>> GetRulesAsync()
        {
            var userId = User.GetUserId();

            var policy = await _validationPolicyProvider.GetAsync(userId);
            if (policy.Count == 0)
                return ValidationRulesNotFound();

            var response = _validationRulesMapper.Map(policy);

            return Ok(response);
        }

        [HttpGet("details")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<VerificationDetailsValidationRuleDto>> GetVerificationDetailsRulesAsync()
        {
            var userId = User.GetUserId();

            var actionTypes = new[] { ActionType.TaxResidence, ActionType.Tin };
            var policy = await _validationPolicyProvider.GetAsync(userId, actionTypes);
            if (policy.Count == 0)
                return ValidationRulesNotFound(actionTypes);

            var response = _validationRulesMapper.MapVerificationDetailsValidationRule(policy);

            return Ok(response);
        }

        [HttpGet("documents/{documentCategory}")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<DocumentValidationRuleDto>> GetDocumentRulesAsync([FromRoute] DocumentCategory documentCategory)
        {
            var userId = User.GetUserId();

            var actionType = _actionTypeMapper.Map(documentCategory, null);
            if (actionType == null)
                return ValidationError($"Not available actions for provided category {documentCategory}");

            var rule = await _validationPolicyProvider.FindAsync(userId, actionType.Value);
            if (rule == null)
                return ValidationRulesNotFound(actionType.Value);

            var response = _validationRulesMapper.MapDocumentValidationRule(actionType.Value, rule);

            return Ok(response);
        }
    }
}