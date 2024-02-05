using System.Collections.Generic;
using System.Net;
using MbDotNet.Models.Responses;
using MbDotNet.Models.Responses.Fields;
using MbDotNet.Models.Stubs;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Models;

namespace WX.B2C.User.Verification.Component.Tests.Mountebank.Extensions
{
    internal static class HttpStubExtensions
    {
        public static HttpStub ReturnsBody(this HttpStub stub, HttpStatusCode statusCode, string rawJsonBody, string decorateFunction) =>
            stub.ReturnsJson(statusCode, rawJsonBody, decorateFunction);

        public static HttpStub ReturnsJson<T>(this HttpStub stub, HttpStatusCode statusCode, T responseObject, string decorateFunction) =>
            stub.Returns(statusCode, 
                         new Dictionary<string, object> { { "Content-Type", "application/json" } }, 
                         responseObject, 
                         decorateFunction: decorateFunction);

        public static HttpStub Returns(
            this HttpStub stub, 
            HttpStatusCode statusCode, 
            IDictionary<string, object> headers,
            object responseObject, 
            string decorateFunction,
            string mode = "text", 
            int? latencyInMilliseconds = null)
        {
            var fields = new HttpResponseFields
            {
                StatusCode = statusCode,
                ResponseObject = responseObject,
                Headers = headers,
                Mode = mode
            };
            var decorator = new Decorator
            {
                DecorateFunction = decorateFunction,
                LatencyInMilliseconds = latencyInMilliseconds
            };

            var response = new IsResponse<HttpResponseFields>(fields, decorator);
            return stub.Returns(response);
        }
    }
}
