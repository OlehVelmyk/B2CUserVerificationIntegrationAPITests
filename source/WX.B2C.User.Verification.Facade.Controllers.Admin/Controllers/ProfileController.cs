using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WX.Admin.Authentication.AccessGroupAuthorization;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Responses;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Services;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware;
using Tasks = WX.B2C.User.Verification.Extensions.TaskExtensions;
using static WX.B2C.User.Verification.Facade.Controllers.Admin.Extensions.Errors;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [CustomAuthorize(AccessGroup.Normal, AccessGroup.OperationControlSecurityLevel)]
    [Route("api/v{version:apiVersion}/verification/{userId}/profile")]
    public class ProfileController : ApiController
    {
        private readonly IProfileStorage _profileStorage;
        private readonly IProfileService _profileService;
        private readonly IInitiationProvider _initiationProvider;
        private readonly IVerificationDetailsMapper _verificationDetailsMapper;
        private readonly IPersonalDetailsMapper _personalDetailsMapper;

        public ProfileController(IProfileStorage profileStorage,
                                 IProfileService profileService,
                                 IInitiationProvider initiationProvider,
                                 IVerificationDetailsMapper verificationDetailsMapper,
                                 IPersonalDetailsMapper personalDetailsMapper)
        {
            _profileStorage = profileStorage ?? throw new ArgumentNullException(nameof(profileStorage));
            _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
            _initiationProvider = initiationProvider ?? throw new ArgumentNullException(nameof(initiationProvider));
            _verificationDetailsMapper = verificationDetailsMapper ?? throw new ArgumentNullException(nameof(verificationDetailsMapper));
            _personalDetailsMapper = personalDetailsMapper ?? throw new ArgumentNullException(nameof(personalDetailsMapper));
        }

        [HttpGet("")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ProfileDto>> GetAsync([FromRoute] Guid userId)
        {
            var (verificationDetails, personalDetails) = await Tasks.WhenAll(
                _profileStorage.FindVerificationDetailsAsync(userId),
                _profileStorage.FindPersonalDetailsAsync(userId));

            if (verificationDetails == null && personalDetails == null)
                return ProfileNotFound();

            var response = new ProfileDto
            {
                UserId = userId,
                PersonalDetails = _personalDetailsMapper.SafeMap(personalDetails),
                VerificationDetails = _verificationDetailsMapper.SafeMap(verificationDetails)
            };
            return Ok(response);
        }

        [HttpPatch("verification-details")]
        [CustomAuthorize(AccessGroup.ComplianceSecurityLevel, AccessGroup.OperationControlSecurityLevel)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ProfileDto>> UpdateAsync([FromRoute] Guid userId,
                                                                [FromBody] UpdateVerificationDetailsRequest request)
        {
            var initiation = _initiationProvider.Create(request.Reason);
            var verificationDetailsPatch = _verificationDetailsMapper.Map(request);
            await _profileService.UpdateAsync(userId, verificationDetailsPatch, initiation);

            return await GetAsync(userId);
        }
    }
}