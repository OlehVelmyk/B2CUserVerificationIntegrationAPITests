using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Requests;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Responses;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware;

namespace WX.B2C.User.Verification.Facade.Controllers.Internal.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/verification/{userId}/checks")]
    public class ChecksController : ApiController
    {
        private readonly ICheckService _checkService;

        public ChecksController(ICheckService checkService)
        {
            _checkService = checkService ?? throw new ArgumentNullException(nameof(checkService));
        }

        [HttpPost("facial-similarity")]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RunFacialSimilarityCheckAsync([FromRoute] Guid userId,
                                                                       [FromBody] FacialSimilarityCheckRequest request)
        {
            var variantId = SelectCheckVariantId(request.Variant).ToGuid();

            var checkRequest = new Core.Contracts.Dtos.NewCheckDto
            {
                CheckType = CheckType.FacialSimilarity,
                Provider = CheckProviderType.Onfido,
                VariantId = variantId,
                RelatedTasks = Array.Empty<Guid>()
            };

            var reason = "Facial similarity confirmation requested.";
            var initiation = Core.Contracts.Dtos.InitiationDto.CreateSystem(reason);

            await _checkService.RequestAsync(
                 userId,
                 checkRequest,
                 initiation);

            return NoContent();
        }

        private static string SelectCheckVariantId(FacialSimilarityCheckVariant variant)
        {
            return variant switch
            {
                FacialSimilarityCheckVariant.Standard => "29AAC87B-3AD4-40E0-B34F-3685CA64805D",
                FacialSimilarityCheckVariant.Video => "23714F13-CBF6-41A4-85C6-719991E6C3F3",
                _ => throw new ArgumentOutOfRangeException(nameof(variant), variant, "Unsupported check variant.")
            };
        }
    }
}