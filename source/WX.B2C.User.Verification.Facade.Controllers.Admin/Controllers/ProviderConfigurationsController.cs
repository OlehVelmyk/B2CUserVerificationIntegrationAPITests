using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WX.Admin.Authentication.AccessGroupAuthorization;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Responses;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware;
using static WX.B2C.User.Verification.Facade.Controllers.Admin.Extensions.Errors;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [CustomAuthorize(AccessGroup.OperationControlSecurityLevel)]
    [Route("api/v{version:apiVersion}/verification/provider-configurations/{providerType}")]
    public class ProviderConfigurationsController : ApiController
    {
        private readonly IBridgerCredentialsService _bridgerCredentialsService;

        public ProviderConfigurationsController(IBridgerCredentialsService bridgerCredentialsService)
        {
            _bridgerCredentialsService = bridgerCredentialsService ?? throw new ArgumentNullException(nameof(bridgerCredentialsService));
        }

        [HttpPut("credentials")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdateCredentialsAsync([FromRoute] ExternalProviderType providerType,
                                                               [FromBody] UpdateCredentialsRequest request)
        {
            if (providerType != ExternalProviderType.LexisNexis)
                return ValidationError($"Credentials can be changed only for {ExternalProviderType.LexisNexis}");

            await _bridgerCredentialsService.UpdateAsync(request.UserId, request.NewPassword, request.Propagate.GetValueOrDefault());
            return NoContent();
        }
    }
}
