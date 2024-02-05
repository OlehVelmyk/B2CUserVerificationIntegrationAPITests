using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using WX.B2C.User.Verification.Facade.Controllers.Internal;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger;

namespace WX.B2C.User.Verification.Api.Internal.Extensions
{
    internal static class SwaggerServiceCollectionExtensions
    {
        public static void AddSwagger(this IServiceCollection services, string projectName)
        {
            services.AddSwaggerGen(c =>
            {
                var buildServiceProvider = services.BuildServiceProvider();
                var provider = buildServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();

                // Add a swagger document for each discovered API version  
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerDoc(description.GroupName, new OpenApiInfo
                    {
                        Title = projectName,
                        Version = description.ApiVersion.ToString(),
                        Description = description.IsDeprecated ? $"{projectName} - DEPRECATED" : projectName
                    });
                }

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.SchemaFilter<RequiredSchemaFilter>();
                c.SchemaFilter<EnumSchemaFilter>();
                c.DocumentFilter<AdditionalPropertiesDocumentFilter>();
                c.DocumentFilter<DocumentHeadersFilter>(GetDocumentParameters());
                c.DocumentFilter<NotNullableArrayFilter>();
                c.OperationFilter<RequiredHeadersFilter>(GetReferenceParameters());
                c.OperationFilter<ResponseBodyFilter>();
                c.OperationFilter<ParameterDuplicationFilter>();
                c.RequestBodyFilter<RequestBodyFilter>();
                c.ParameterFilter<ExplodingQueryParameterFilter>();
                c.ParameterFilter<RequiredParameterFilter>();
                c.ParameterFilter<StringEnumParameterFilter>();

                c.CustomOperationIds(apiDesc =>
                {
                    if (apiDesc.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                        return $"{controllerActionDescriptor.ControllerName}_{controllerActionDescriptor.ActionName}";

                    return apiDesc.ActionDescriptor.Id;
                });
            });
        }

        private static IEnumerable<OpenApiReference> GetReferenceParameters() =>
            GetDocumentParameters()
                .Select(x => x.Reference);

        private static IEnumerable<OpenApiParameter> GetDocumentParameters()
        {
            yield return new OpenApiParameter
            {
                Reference = new OpenApiReference
                {
                    Id = Constants.Headers.CorrelationIdHeaderId,
                    Type = ReferenceType.Parameter
                },
                Name = "correlationId",
                AllowEmptyValue = false,
                Description =
                    "Unique identifier value that is attached to requests and messages that allow reference to a particular transaction or event chain",
                In = ParameterLocation.Header,
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Format = "uuid",
                }
            };

            yield return new OpenApiParameter
            {
                Reference = new OpenApiReference
                {
                    Id = Constants.Headers.OperationIdHeaderId,
                    Type = ReferenceType.Parameter
                },
                Name = "operationId",
                AllowEmptyValue = false,
                Description = "Request id/operation id to identify operation ordering in a chain",
                In = ParameterLocation.Header,
                Required = false,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Format = "uuid",
                }
            };
        }
    }
}