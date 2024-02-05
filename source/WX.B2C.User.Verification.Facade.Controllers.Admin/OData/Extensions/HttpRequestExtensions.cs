using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.OData.Extensions
{
    internal static class HttpRequestExtensions
    {
        private const string Skip = "$skip";
        private const string Top = "$top";

        public static string GetNextPageLink(this HttpRequest request, int? skipped, int taken, int? total)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));
            if (request.Query is null)
                throw new ArgumentNullException(nameof(request.Query));

            var totalSkipped = skipped.GetValueOrDefault() + taken;

            if (total is null)
                return null;
            if (totalSkipped >= total)
                return null;

            var query = request.Query.Where(kvp => kvp.Key != Skip && kvp.Key != Top);

            query = query.Append(KeyValuePair.Create(Skip, new StringValues(totalSkipped.ToString())));
            query = query.Append(KeyValuePair.Create(Top, new StringValues(Math.Min(total.Value - totalSkipped, taken).ToString())));

            var uriBuilder = new UriBuilder(request.Scheme, request.Host.Host)
            {
                Path = (request.PathBase + request.Path).ToUriComponent(),
                Query = string.Join('&', query.Select(PrepareQueryParameter))
            };
            if (request.Host.Port.HasValue)
                uriBuilder.Port = request.Host.Port.Value;

            return uriBuilder.Uri.ToString();
        }

        private static string PrepareQueryParameter(KeyValuePair<string, StringValues> kvp)
        {
            var key = kvp.Key.StartsWith('$')
                ? '$' + Uri.EscapeDataString(kvp.Key[1..])
                : Uri.EscapeDataString(kvp.Key);

            var value = Uri.EscapeDataString(kvp.Value);
            return $"{key}={value}";
        }
    }
}
