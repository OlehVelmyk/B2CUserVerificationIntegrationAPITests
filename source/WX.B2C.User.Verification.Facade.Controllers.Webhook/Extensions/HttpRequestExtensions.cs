using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WX.B2C.User.Verification.Facade.Controllers.Webhook.Extensions
{
    internal static class HttpRequestExtensions
    {
        public static async Task<string> GetRawBodyStringAsync(this HttpRequest request, Encoding encoding = null)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (encoding == null)
                encoding = Encoding.UTF8;

            request.EnableBuffering();

            using var reader = new StreamReader(request.Body, encoding, true, 1024, true);
            var rawBody = await reader.ReadToEndAsync();

            request.Body.Seek(0, SeekOrigin.Begin);

            return rawBody;
        }
    }
}
