using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WX.B2C.User.Verification.IpStack
{
    internal class ResponseCodeAdapter : DelegatingHandler
    {
        /// <summary>
        /// For some reason IpStack always returns status ok.
        /// 502 Bad Gateway is not a part of known exceptions according to documentation:https://ipstack.com/documentation#errors
        /// Instead of normal error response IpStack returns:
        /// <html>
        /// <head><title>502 Bad Gateway</title></head>
        /// <body>
        /// <center><h1>502 Bad Gateway</h1></center>
        /// </body>
        /// </html>
        /// </summary>
        private static readonly Dictionary<string, HttpStatusCode> StatusCodeMapping = new()
        {
            { "500 Internal Server Error", HttpStatusCode.InternalServerError },
            { "501 Not Implemented", HttpStatusCode.NotImplemented },
            { "502 Bad Gateway", HttpStatusCode.BadGateway },
            { "503 Service Unavailable", HttpStatusCode.ServiceUnavailable },
            { "504 Gateway Timeout", HttpStatusCode.GatewayTimeout },
        };

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return response;

            var content = await response.Content.ReadAsStringAsync();
            response.StatusCode = ParseStatusCode(content, response.StatusCode);
            return response;
        }

        private static HttpStatusCode ParseStatusCode(string content, HttpStatusCode originCode)
        {
            foreach (var (message, code) in StatusCodeMapping)
            {
                if (content.Contains(message))
                    return code;
            }
            return originCode;
        }
    }
}