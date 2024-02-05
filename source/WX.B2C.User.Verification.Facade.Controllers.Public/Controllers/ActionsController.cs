using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Facade.Controllers.Public.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Public.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Public.Mappers;
using WX.B2C.User.Verification.Facade.Controllers.Public.Responses;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/verification/actions")]
    public class ActionsController : ApiController
    {
        private readonly IActionService _actionService;
        private readonly IActionMapper _actionMapper;

        public ActionsController(IActionService actionService, IActionMapper actionMapper)
        {
            _actionService = actionService ?? throw new ArgumentNullException(nameof(actionService));
            _actionMapper = actionMapper ?? throw new ArgumentNullException(nameof(actionMapper));
        }

        [HttpGet]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserActionDto[]>> GetAsync()
        {
            var userId = User.GetUserId();

            var actions = await _actionService.GetAsync(userId);
            var response = actions.Select(_actionMapper.Map).ToArray();

            return Ok(response);
        }
    }
}