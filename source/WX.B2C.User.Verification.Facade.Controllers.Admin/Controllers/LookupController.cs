using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WX.Admin.Authentication.AccessGroupAuthorization;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Responses;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [CustomAuthorize(AccessGroup.Normal, AccessGroup.OperationControlSecurityLevel)]
    [Route("api/v{version:apiVersion}/verification/lookup")]
    public class LookupController : ApiController
    {
        private readonly ISurveyTemplatesProvider _templatesProvider;
        private readonly IDocumentTypeProvider _documentTypeProvider;
        private readonly ISurveyMapper _surveyMapper;
        private readonly IDocumentCategoryMapper _documentCategoryMapper;

        public LookupController(ISurveyTemplatesProvider templatesProvider,
                                IDocumentTypeProvider documentTypeProvider,
                                ISurveyMapper surveyMapper,
                                IDocumentCategoryMapper documentCategoryMapper)
        {
            _templatesProvider = templatesProvider ?? throw new ArgumentNullException(nameof(templatesProvider));
            _documentTypeProvider = documentTypeProvider ?? throw new ArgumentNullException(nameof(documentTypeProvider));
            _surveyMapper = surveyMapper ?? throw new ArgumentNullException(nameof(surveyMapper));
            _documentCategoryMapper = documentCategoryMapper ?? throw new ArgumentNullException(nameof(documentCategoryMapper));
        }

        [HttpGet("survey-templates")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<SurveyTemplateDto[]>> GetSurveyTemplates()
        {
            var surveys = await _templatesProvider.GetAsync();
            return Ok(surveys.Select(_surveyMapper.Map));
        }

        [HttpGet("document-categories")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<DocumentCategoryLookupDto[]> GetDocumentCategories()
        {
            var documentCategoriesInfo = _documentTypeProvider.Get();
            return Ok(documentCategoriesInfo.Select(pair => _documentCategoryMapper.Map(pair.Key, pair.Value)).ToArray());
        }
    }
}
