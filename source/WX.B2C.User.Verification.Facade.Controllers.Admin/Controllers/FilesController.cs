using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WX.Admin.Authentication.AccessGroupAuthorization;
using WX.B2C.User.Verification.Core.Contracts.Providers;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Responses;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware;
using static WX.B2C.User.Verification.Facade.Controllers.Admin.Extensions.Errors;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [CustomAuthorize(AccessGroup.Normal, AccessGroup.OperationControlSecurityLevel)]
    [Route("api/v{version:apiVersion}/verification/{userId}/files")]
    public class FilesController : ApiController
    {
        private readonly IFileStorage _fileStorage;
        private readonly IFileProvider _fileProvider;
        private readonly IContentTypeMapper _contentTypeMapper;

        public FilesController(IFileStorage fileStorage,
                               IFileProvider fileProvider,
                               IContentTypeMapper contentTypeMapper)
        {
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
            _contentTypeMapper = contentTypeMapper ?? throw new ArgumentNullException(nameof(contentTypeMapper));
        }

        [HttpGet("{fileId}/download")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DownloadAsync([FromRoute] Guid userId, [FromRoute] Guid fileId)
        {
            var file = await _fileStorage.FindAsync(userId, fileId);
            if (file == null)
                return FileNotFound();

            var stream = await _fileProvider.DownloadAsync(file);

            var contentType = _contentTypeMapper.Map(file.FileName);
            return new FileStreamResult(stream, contentType);
        }
    }
}
