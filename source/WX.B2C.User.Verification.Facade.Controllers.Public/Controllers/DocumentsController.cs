using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Core.Contracts.Providers;
using WX.B2C.User.Verification.Facade.Controllers.Public.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Public.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Public.Filters;
using WX.B2C.User.Verification.Facade.Controllers.Public.Mappers;
using WX.B2C.User.Verification.Facade.Controllers.Public.Requests;
using WX.B2C.User.Verification.Facade.Controllers.Public.Responses;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/verification/documents")]
    public class DocumentsController : ApiController
    {
        private readonly IFileService _fileService;
        private readonly IFileProvider _fileProvider;
        private readonly IFileMapper _fileMapper;
        private readonly IDocumentService _documentService;
        private readonly IDocumentMapper _documentMapper;

        public DocumentsController(
            IFileService fileService, 
            IFileProvider fileProvider,
            IFileMapper fileMapper,
            IDocumentService documentService,
            IDocumentMapper documentMapper)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
            _fileMapper = fileMapper ?? throw new ArgumentNullException(nameof(fileMapper));
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
            _documentMapper = documentMapper ?? throw new ArgumentNullException(nameof(documentMapper));
        }

        [HttpPost("files")]
        [ValidateAsync(typeof(UploadDocumentFileRequest))]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UploadedFileDto>> UploadFilesAsync([FromForm] UploadDocumentFileRequest request)
        {
            var userId = User.GetUserId();
            var uploadedFileDto = _fileMapper.Map(request.File);
            var fileId = await _fileService.UploadAsync(userId, uploadedFileDto);
            return Ok(new UploadedFileDto { FileId = fileId });
        }

        [HttpPost("")]
        [ValidateAsync(typeof(SubmitDocumentRequest))]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SubmitAsync([FromBody] SubmitDocumentRequest request)
        {
            var userId = User.GetUserId();

            var fileIds = request.Provider.HasValue
                ? await UploadExternalFilesAsync(userId, request.Files, request.Provider.Value, request.Type).ToArrayAsync()
                : request.Files.Select(Guid.Parse).ToArray();

            var initiation = Core.Contracts.Dtos.InitiationDto.CreateUser();
            var submitDocumentDto = _documentMapper.Map(request, fileIds);
            await _documentService.SubmitAsync(userId, submitDocumentDto, initiation);
            return Ok();
        }

        private async IAsyncEnumerable<Guid> UploadExternalFilesAsync(Guid userId, IEnumerable<string> fileIds, 
                                                                      ExternalFileProviderType provider, string type)
        {
            foreach (var externalFileId in fileIds)
            {
                var externalFile = _fileMapper.Map(externalFileId, provider, type);
                var downloadedFile = await _fileProvider.DownloadAsync(userId, externalFile);

                var uploadedFile = new Core.Contracts.Dtos.UploadedFileDto
                {
                    File = downloadedFile.Data,
                    Name = downloadedFile.Name,
                    ExternalData = new Core.Contracts.Dtos.ExternalFileData
                    {
                        Id = externalFileId,
                        Provider = provider
                    }
                };
                yield return await _fileService.UploadAsync(userId, uploadedFile);
            }
        }
    }
}