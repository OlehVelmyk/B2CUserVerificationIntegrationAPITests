using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Public.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Public.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Public.Filters;
using WX.B2C.User.Verification.Facade.Controllers.Public.Mappers;
using WX.B2C.User.Verification.Facade.Controllers.Public.Requests;
using WX.B2C.User.Verification.Facade.Controllers.Public.Responses;
using WX.B2C.User.Verification.Facade.Controllers.Public.Services;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/verification/profile")]
    public class ProfileController : ApiController
    {
        private readonly IProfileService _profileService;
        private readonly IProfileAggregationService _profileAggregationService;
        private readonly IVerificationDetailsMapper _verificationDetailsMapper;

        public ProfileController(
            IProfileService profileService,
            IProfileAggregationService profileAggregationService,
            IVerificationDetailsMapper verificationDetailsMapper)
        {
            _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
            _profileAggregationService = profileAggregationService ?? throw new ArgumentNullException(nameof(profileAggregationService));
            _verificationDetailsMapper = verificationDetailsMapper ?? throw new ArgumentNullException(nameof(verificationDetailsMapper));
        }

        [HttpGet("")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ProfileDto>> GetAsync()
        {
            var userId = User.GetUserId();

            var response = await _profileAggregationService.AggregateAsync(userId);

            return Ok(response);
        }

        [HttpPatch("details")]
        [ValidateAsync(typeof(UpdateVerificationDetailsRequest))]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ProfileDto>> UpdateAsync([FromBody] UpdateVerificationDetailsRequest request)
        {
            var userId = User.GetUserId();

            var verificationDetailsPatch = _verificationDetailsMapper.Map(request);
            var initiation = InitiationDto.CreateUser();
            await _profileService.UpdateAsync(userId, verificationDetailsPatch, initiation);

            return await GetAsync();
        }
    }
}
