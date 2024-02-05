using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Mappers;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Responses;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware;
using static WX.B2C.User.Verification.Facade.Controllers.Internal.Extensions.Errors;

namespace WX.B2C.User.Verification.Facade.Controllers.Internal.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/verification/{userId}/profile")]
    public class ProfileController : ApiController
    {
        private readonly IProfileStorage _profileStorage;
        private readonly IVerificationDetailsMapper _verificationDetailsMapper;
        private readonly ILogger _logger;

        public ProfileController(IProfileStorage profileStorage,
                                 IVerificationDetailsMapper verificationDetailsMapper,
                                 ILogger logger)
        {
            _profileStorage = profileStorage ?? throw new ArgumentNullException(nameof(profileStorage));
            _verificationDetailsMapper = verificationDetailsMapper ?? throw new ArgumentNullException(nameof(verificationDetailsMapper));
            _logger = logger?.ForContext<ProfileController>() ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ProfileDto>> GetAsync([FromRoute] Guid userId)
        {
            var verificationDetails = await _profileStorage.FindVerificationDetailsAsync(userId);
            if (verificationDetails == null) 
                return ProfileNotFound(userId);

            var response = new ProfileDto
            {
                UserId = userId,
                VerificationDetails = _verificationDetailsMapper.ToDto(verificationDetails)
            };
            return Ok(response);
        }

        [HttpDelete("")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public Task<IActionResult> DeleteAsync([FromRoute] Guid userId)
        {
            _logger.Information("User {UserId} should be deleted.", userId);

            var response = (IActionResult) NoContent();
            return Task.FromResult(response);
        }
    }
}