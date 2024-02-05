using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WX.B2C.User.Verification.Core.Contracts.Providers;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Mappers;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Responses;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware;
using static WX.B2C.User.Verification.Facade.Controllers.Internal.Extensions.Errors;

namespace WX.B2C.User.Verification.Facade.Controllers.Internal.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/verification/{userId}/documents")]
    public class DocumentsController : ApiController
    {
        private readonly IDocumentStorage _documentStorage;
        private readonly IFileProvider _fileProvider;
        private readonly IDocumentMapper _documentMapper;

        public DocumentsController(IDocumentStorage documentStorage,
                                   IFileProvider fileProvider,
                                   IDocumentMapper documentMapper)
        {
            _documentStorage = documentStorage ?? throw new ArgumentNullException(nameof(documentStorage));
            _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
            _documentMapper = documentMapper ?? throw new ArgumentNullException(nameof(documentMapper));
        }

        [HttpGet("{category}/latest")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<DocumentDto>> GetLatestAsync([FromRoute] Guid userId,
                                                                    [FromRoute] DocumentCategory category)
        {
            var document = await _documentStorage.FindLatestAsync(userId, category);
            if (document is null)
                return DocumentNotFound(userId, category);


            var hrefs = await document.Files.Foreach(GenerateDownloadHrefAsync);
            return Ok(_documentMapper.Map(document, hrefs));

            async Task<(Guid Key, string Value)> GenerateDownloadHrefAsync(Core.Contracts.Dtos.DocumentFileDto file) =>
                (file.FileId, await _fileProvider.GenerateDownloadHrefAsync(userId, file));
        }
    }
}