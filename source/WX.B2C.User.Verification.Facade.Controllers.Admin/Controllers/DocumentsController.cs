using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WX.Admin.Authentication.AccessGroupAuthorization;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Providers;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Responses;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Services;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;
using static WX.B2C.User.Verification.Facade.Controllers.Admin.Extensions.Errors;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [CustomAuthorize(AccessGroup.Normal, AccessGroup.OperationControlSecurityLevel)]
    [Route("api/v{version:apiVersion}/verification/{userId}/documents")]
    public class DocumentsController : ApiController
    {
        private readonly IDocumentService _documentService;
        private readonly IDocumentStorage _documentStorage;
        private readonly IDocumentMapper _documentMapper;
        private readonly IFileService _fileService;
        private readonly IFileStorage _fileStorage;
        private readonly IFileProvider _fileProvider;
        private readonly IFileMapper _fileMapper;
        private readonly IInitiationProvider _initiationProvider;

        public DocumentsController(IDocumentService documentService,
                                   IDocumentStorage documentStorage,
                                   IDocumentMapper documentMapper,
                                   IFileService fileService, 
                                   IFileStorage fileStorage,
                                   IFileProvider fileProvider,
                                   IFileMapper fileMapper,
                                   IInitiationProvider initiationProvider)
        {
            _documentStorage = documentStorage ?? throw new ArgumentNullException(nameof(documentStorage));
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
            _documentMapper = documentMapper ?? throw new ArgumentNullException(nameof(documentMapper));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
            _fileMapper = fileMapper ?? throw new ArgumentNullException(nameof(fileMapper));
            _initiationProvider = initiationProvider ?? throw new ArgumentNullException(nameof(initiationProvider));
        }

        [HttpGet("")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<DocumentDto[]>> GetAllAsync([FromRoute] Guid userId,
                                                                   [FromQuery][NotRequired] DocumentCategory? category)
        {
            var documents = await _documentStorage.FindAsync(userId, category);
            return Ok(documents.Select(_documentMapper.Map));
        }

        [HttpGet("{documentId}")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<DocumentDto>> GetAsync([FromRoute] Guid userId,
                                                              [FromRoute] Guid documentId)
        {
            var document = await _documentStorage.FindAsync(documentId, userId);

            if (document == null)
                return DocumentNotFound();

            var result = _documentMapper.Map(document);
            return Ok(result);
        }

        [HttpPost("files")]
        [CustomAuthorize(AccessGroup.ComplianceSecurityLevel, AccessGroup.OperationControlSecurityLevel)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<UploadedFileDto>> UploadAsync([FromRoute] Guid userId,
                                                                     [FromForm] UploadDocumentFileRequest request)
        {
            var uploadedFileDto = _fileMapper.Map(request.File);

            var fileId = await _fileService.UploadAsync(userId, uploadedFileDto);

            if (request.UploadToProvider)
            {
                var file = await _fileStorage.FindAsync(userId, fileId);
                if (file.Provider == null)
                {
                    var uploadFileDto = _fileMapper.Map(request);
                    var externalId = await _fileProvider.UploadAsync(userId, uploadFileDto);
                    var externalData = new Core.Contracts.Dtos.ExternalFileData { Id = externalId, Provider = uploadFileDto.Provider };
                    await _fileService.UpdateAsync(userId, fileId, externalData);
                }
                else if (file.Provider != request.Provider)
                    return FileUploadedToOtherProvider(file.Provider.Value);
            }

            return Ok(new UploadedFileDto { FileId = fileId });
        }

        [HttpPost("")]
        [CustomAuthorize(AccessGroup.ComplianceSecurityLevel, AccessGroup.OperationControlSecurityLevel)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SubmitAsync([FromRoute] Guid userId,
                                                     [FromBody] SubmitDocumentRequest request)
        {
            var submitDocumentDto = _documentMapper.Map(request);
            var initiation = _initiationProvider.Create(request.Reason);
            await _documentService.SubmitAsync(userId, submitDocumentDto, initiation);
            return Ok();
        }
    }
}