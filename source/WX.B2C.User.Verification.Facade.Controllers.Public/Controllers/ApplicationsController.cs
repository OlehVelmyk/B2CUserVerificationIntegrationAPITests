using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Providers;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Facade.Controllers.Public.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Public.Filters;
using WX.B2C.User.Verification.Facade.Controllers.Public.Providers;
using WX.B2C.User.Verification.Facade.Controllers.Public.Requests;
using WX.B2C.User.Verification.Facade.Controllers.Public.Responses;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware;
using static WX.B2C.User.Verification.Facade.Controllers.Public.Extensions.Errors;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/verification/applications")]
    public class ApplicationsController : ApiController
    {
        private readonly IApplicationStorage _applicationStorage;
        private readonly IVerificationPolicyProvider _policyProvider;
        private readonly IFeatureToggleService _featureToggleService;
        private readonly IPolicySelectionContextProvider _policyContextProvider;
        private readonly IApplicationService _applicationService;

        public ApplicationsController(IFeatureToggleService featureToggleService,
                                      IApplicationStorage applicationStorage,
                                      IVerificationPolicyProvider policyProvider,
                                      IPolicySelectionContextProvider policyContextProvider,
                                      IApplicationService applicationService)
        {
            _featureToggleService = featureToggleService ?? throw new ArgumentNullException(nameof(featureToggleService));
            _applicationStorage = applicationStorage ?? throw new ArgumentNullException(nameof(applicationStorage));
            _policyProvider = policyProvider ?? throw new ArgumentNullException(nameof(policyProvider));
            _policyContextProvider = policyContextProvider ?? throw new ArgumentNullException(nameof(policyContextProvider));
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
        }

        [HttpPost("")]
        [CaptureIpAddress]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RegisterAsync(RegisterApplicationRequest request)
        {
            var userId = User.GetUserId();
            var productType = request.ProductType ?? ProductType.WirexBasic;

            var applicationId = await _applicationStorage.FindIdAsync(userId, productType);
            if (applicationId.HasValue)
                return ApplicationAlreadyCreated(userId, applicationId.Value);

            var verificationContext = await _policyContextProvider.GetVerificationContextAsync(userId);
            if (verificationContext.Country is null)
                return ResidenceAddressNotFound(userId);

            var isAvailable = await _featureToggleService.IsVerificationAvailableAsync(verificationContext);
            if (!isAvailable)
                return ApplicationNotAvailable(verificationContext.Country, verificationContext.State);

            var policyId = await _policyProvider.GetAsync(verificationContext);

            var newApplication = new NewVerificationApplicationDto { ProductType = productType, PolicyId = policyId };
            await _applicationService.RegisterAsync(userId, newApplication, InitiationDto.CreateUser());
            return Ok();
        }

    }
}