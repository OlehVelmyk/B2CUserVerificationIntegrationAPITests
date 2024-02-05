using System;
using System.Text;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Infrastructure.Remoting.FabricWrappers
{
    public static class ServiceRemotingRequestMessageExtensions
    {
        public static OperationContext ToOperationContext(this IServiceRemotingRequestMessage message)
        {
            var headers = message.GetHeader();

            Guid correlationId;
            Guid? parentOperationId = null;

            if (headers.TryGetHeaderValue(Constants.Headers.OperationId, out var operationIdData))
                parentOperationId = new Guid(operationIdData);

            if (headers.TryGetHeaderValue(Constants.Headers.CorrelationId, out var correlationIdData))
                correlationId = new Guid(correlationIdData);
            else
                correlationId = Guid.Empty;

            var operationId = Guid.NewGuid();
            var operationName = $"RPC: {headers.MethodName}";

            return OperationContext.Create(correlationId, parentOperationId, operationId, operationName);
        }

        public static void PopulateFromOperationContext(this IServiceRemotingRequestMessage requestMessage, OperationContext context)
        {
            if (requestMessage == null)
                throw new ArgumentNullException(nameof(requestMessage));

            if (context == null)
                return;

            var header = requestMessage.GetHeader();

            header.SetPropertyIfNotEmpty(Constants.Headers.OperationId, context.OperationId)
                  .SetPropertyIfNotEmpty(Constants.Headers.CorrelationId, context.CorrelationId)
                  .SetPropertyIfNotEmpty(Constants.Headers.OperationName, context.OperationName);
        }

        private static IServiceRemotingRequestMessageHeader SetPropertyIfNotEmpty(this IServiceRemotingRequestMessageHeader header, string name, string value)
        {
            if (header == null)
                throw new ArgumentNullException(nameof(header));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (header.AlreadyHasProperty(name))
                return header;

            if (!string.IsNullOrEmpty(value))
                header.AddHeader(name, Encoding.UTF8.GetBytes(value));

            return header;
        }

        private static IServiceRemotingRequestMessageHeader SetPropertyIfNotEmpty(this IServiceRemotingRequestMessageHeader header, string name, Guid value)
        {
            if (header == null)
                throw new ArgumentNullException(nameof(header));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (header.AlreadyHasProperty(name))
                return header;

            if (value != default)
                header.AddHeader(name, value.ToByteArray());

            return header;
        }

        private static bool AlreadyHasProperty(this IServiceRemotingRequestMessageHeader header, string name)
        {
            if (!header.TryGetHeaderValue(name, out var headerData))
                return false;

            if (headerData == default)
                throw new ArgumentNullException(nameof(headerData));

            return true;
        }
    }
}
