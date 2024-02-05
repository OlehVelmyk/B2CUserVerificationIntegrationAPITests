using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Extensions;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger
{
    /// <summary>
    /// Motivation:
    /// The only one working solution was found which allows to pass array of values
    /// to API method in query parameters and which is correctly recognized by Autorest.
    /// This solution implies that values would be passed in next way: colors=blue&colors=black&colors=brown 
    /// which corresponds to 'style: form' and 'explode: true' according to swagger specification.
    /// However, Swashbuckle places 'explode' property to swagger.json at a different level
    /// which does not satisfy Autorest expectations.
    /// Therefore, we should add 'explode' property manually using parameter extensions
    /// but in addition we should also specify defined 'explode' parameter option to 'true'
    /// because otherwise 'Duplication error' occurs on the Swagger page.
    /// </summary>
    public class ExplodingQueryParameterFilter : IParameterFilter
    {
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            if (parameter.In == ParameterLocation.Query && parameter.Schema.Type == "array")
            {
                parameter.Style = ParameterStyle.Form;
                parameter.Explode = true;
                parameter.Extensions.Add(new ExplodeParameterApiExtension());
            }
        }
    }
}
