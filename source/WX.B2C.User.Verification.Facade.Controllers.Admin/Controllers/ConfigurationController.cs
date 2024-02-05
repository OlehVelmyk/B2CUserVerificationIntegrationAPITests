using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WX.Admin.Authentication.AccessGroupAuthorization;
using WX.B2C.User.Verification.Configuration;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [CustomAuthorize(AccessGroup.OperationControlSecurityLevel)]
    [Route("api/v{version:apiVersion}/verification/configurations")]
    public class ConfigurationController : ApiController
    {
        private readonly IConfigurationManager _configurationManager;
        private readonly IConfigurationMapper _configurationMapper;

        public ConfigurationController(IConfigurationManager configurationManager,
                                       IConfigurationMapper configurationMapper)
        {
            _configurationManager = configurationManager ?? throw new ArgumentNullException(nameof(configurationManager));
            _configurationMapper = configurationMapper ?? throw new ArgumentNullException(nameof(configurationMapper));
        }

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<ServiceConfigurationDto> Get()
        {
            var serviceConfigurations = _configurationManager.Get();
            var response = _configurationMapper.Map(serviceConfigurations);
            return Ok(response);
        }

        [CustomAuthorize(AccessGroup.ArchitectSecurityLevel)]
        [HttpPatch("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ServiceConfigurationDto>> UpdateAsync([FromBody] UpdateConfigurationRequest configurationDto)
        {
            var patch = _configurationMapper.Map(configurationDto);
            await _configurationManager.UpdateAsync(patch);

            return Get();
        }
    }
}
