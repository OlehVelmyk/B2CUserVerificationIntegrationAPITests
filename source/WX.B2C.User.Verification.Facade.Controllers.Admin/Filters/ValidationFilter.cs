using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Responses;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var validationErrors = context.ModelState
                                              .Where(x => x.Value.Errors.Any())
                                              .SelectMany(x => x.Value.Errors.Select(error => (x.Key, error)))
                                              .Select(error => ErrorDetails.CreateValidationError($"{error.Key}:{error.error.ErrorMessage}"))
                                              .ToArray();

                var errorResponse = ErrorResponse.Create(validationErrors);
                context.Result = new BadRequestObjectResult(errorResponse);

                return;
            }

            await next();
        }
    }
}
