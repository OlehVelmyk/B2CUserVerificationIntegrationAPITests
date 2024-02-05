using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WX.Admin.Authentication.AccessGroupAuthorization;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Responses;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Services;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware;
using static WX.B2C.User.Verification.Facade.Controllers.Admin.Extensions.Errors;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [CustomAuthorize(AccessGroup.Normal, AccessGroup.OperationControlSecurityLevel)]
    [Route("api/v{version:apiVersion}/verification/notes")]
    public class NotesController : ApiController
    {
        private readonly INoteStorage _noteStorage;
        private readonly INoteService _notesService;
        private readonly INoteMapper _noteMapper;
        private readonly IInitiationProvider _initiationProvider;

        public NotesController(
            INoteStorage noteStorage,
            INoteService notesService,
            INoteMapper noteMapper,
            IInitiationProvider initiationProvider)
        {
            _noteStorage = noteStorage ?? throw new ArgumentNullException(nameof(noteStorage));
            _notesService = notesService ?? throw new ArgumentNullException(nameof(notesService));
            _noteMapper = noteMapper ?? throw new ArgumentNullException(nameof(noteMapper));
            _initiationProvider = initiationProvider ?? throw new ArgumentNullException(nameof(initiationProvider));
        }

        [HttpGet("{subject}/{subjectId}")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<NoteDto[]>> GetAsync([FromRoute] NoteSubject subject,
                                                            [FromRoute] Guid subjectId)
        {
            var notes = await _noteStorage.GetAsync(subject, subjectId);
            var response = notes.Select(_noteMapper.Map).ToArray();
            return Ok(response);
        }

        [HttpPost]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<NoteDto>> CreateAsync([FromBody] CreateNoteRequest request)
        {
            var authorEmail = _initiationProvider.GetCurrentInitiator();
            var newNoteDto = _noteMapper.Map(request, authorEmail);
            var noteId = await _notesService.CreateAsync(newNoteDto);
            var noteDto = await _noteStorage.FindAsync(noteId);
            return Created(noteId.ToString(), noteDto);
        }

        [HttpDelete("{noteId}")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid noteId)
        {
            var note = await _noteStorage.FindAsync(noteId);
            if (note == null)
                return NoteNotFound();

            await _notesService.DeleteAsync(noteId);
            return NoContent();
        }
    }
}