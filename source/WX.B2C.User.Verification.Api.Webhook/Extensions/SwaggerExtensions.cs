using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace WX.B2C.User.Verification.Api.Webhook.Extensions
{
    internal static class SwaggerExtensions
    {
        public static IEnumerable<UrlDescriptor> BuildSwaggerEndpoints(this IEnumerable<ApiVersionDescription> versionDescriptions, string projectName)
        {
            if (versionDescriptions == null)
                throw new ArgumentNullException(nameof(versionDescriptions));
            if (string.IsNullOrEmpty(projectName))
                throw new ArgumentNullException(nameof(projectName));

            return from description in versionDescriptions
                   let name = $"{projectName} {description.GroupName}"
                   let url = $"./swagger/{description.GroupName}/swagger.json"
                   select new UrlDescriptor { Url = url, Name = name };
        }

        public static void SwaggerEndpoints(this SwaggerUIOptions options, IEnumerable<UrlDescriptor> endpoints)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (endpoints == null)
                throw new ArgumentNullException(nameof(endpoints));

            foreach (var endpoint in endpoints)
            {
                options.SwaggerEndpoint(endpoint.Url, endpoint.Name);
            }
        }
    }
}
