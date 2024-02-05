using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WX.B2C.User.Verification.Infrastructure.Remoting;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.OperationContext
{
    internal class OperationContextMiddleware
    {
        private const string GuidRegex = "[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}";

        private readonly RequestDelegate _next;
        private readonly IOperationContextScopeFactory _contextScopeFactory;

        public OperationContextMiddleware(RequestDelegate next, IOperationContextScopeFactory contextScopeFactory)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _contextScopeFactory = contextScopeFactory ?? throw new ArgumentNullException(nameof(contextScopeFactory));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = TryReadHeaderValue(context, Constants.Headers.CorrelationId) ?? Guid.NewGuid();
            var parentOperationId = TryReadHeaderValue(context, Constants.Headers.OperationId);
            var operationName = GetOperationName(context);

            using (_contextScopeFactory.Create(correlationId, parentOperationId, operationName))
            {
                await _next.Invoke(context);
            }
        }

        private string GetOperationName(HttpContext context)
        {
            var requestPath = context.Request.Path;
            var maskedPath = Regex.Replace(requestPath, GuidRegex, "*", RegexOptions.IgnoreCase);
            var operationName = $"{context.Request.Method} {maskedPath}";
            return operationName;
        }

        private static Guid? TryReadHeaderValue(HttpContext context, string key)
        {
            var header = context.Request.Headers[key];
            if (header.Count == 0)
                return null;

            if (Guid.TryParse(header[0], out var result))
                return result;

            return null;
        }
    }
}