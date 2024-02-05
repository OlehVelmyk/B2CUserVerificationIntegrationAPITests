using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;
using WX.B2C.User.Verification.Facade.Controllers.Webhook.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Webhook.Services;

namespace WX.B2C.User.Verification.Facade.Controllers.Webhook.Filters
{
    public class WebhookSecretFilterAttribute : TypeFilterAttribute
    {
        public WebhookSecretFilterAttribute()
            : base(typeof(WebhookSecretFilter))
        {
        }
    }

    public class WebhookSecretFilter : IAsyncResourceFilter
    {
        private readonly ISecretValidator _secretValidator;

        public WebhookSecretFilter(ISecretValidator secretValidator)
        {
            _secretValidator = secretValidator ?? throw new ArgumentNullException(nameof(secretValidator));
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var request = context.HttpContext.Request;
            var rawBody = await request.GetRawBodyStringAsync();
            var response = JObject.Parse(rawBody);
            var secret = response.GetValue("secret")?.Value<string>();
            var isSecretValid = _secretValidator.Validate(secret);
            if (!isSecretValid)
                throw new SecretValidationException(secret);

            await next();
        }
    }
}
