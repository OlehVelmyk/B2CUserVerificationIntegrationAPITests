using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WX.Admin.Authentication.AccessGroupAuthorization;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers;
using WX.B2C.User.Verification.Facade.Controllers.Admin.OData.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Responses;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [CustomAuthorize(AccessGroup.Normal, AccessGroup.OperationControlSecurityLevel)]
    [Route("api/v{version:apiVersion}/verification/{userId}/audit-entries")]
    public class AuditEntriesController : ApiController
    {
        private readonly IAuditEntryStorage _auditEntryStorage;
        private readonly IAuditMapper _auditMapper;

        public AuditEntriesController(IAuditEntryStorage auditEntryStorage,
                                      IAuditMapper auditMapper)
        {
            _auditEntryStorage = auditEntryStorage ?? throw new ArgumentNullException(nameof(auditEntryStorage));
            _auditMapper = auditMapper ?? throw new ArgumentNullException(nameof(auditMapper));
        }

        [HttpGet]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PageResult<AuditEntryDto>>> GetAsync([FromRoute] Guid userId, 
                                                                            [FromQuery] ODataQueryParameters queryParameters)
        {
            var oDataQueryContext = _auditMapper.Map(queryParameters);
            var pagedDto = await _auditEntryStorage.FindAsync(userId, oDataQueryContext);

            var result = new PageResult<AuditEntryDto>
            {
                Items = pagedDto.Items.Select(_auditMapper.Map).ToArray(),
                NextPageLink = Request.GetNextPageLink(queryParameters.Skip, pagedDto.Items.Length, pagedDto.Total),
                TotalCount = pagedDto.Total
            };

            return Ok(result);
        }
    }
}