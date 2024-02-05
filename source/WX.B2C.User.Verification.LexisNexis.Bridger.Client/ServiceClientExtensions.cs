using System;
using System.ServiceModel;
using System.Threading.Tasks;
using BridgerReference;
using Polly;
using WX.B2C.User.Verification.LexisNexis.Bridger.Client.Exceptions;
using static WX.B2C.User.Verification.LexisNexis.Bridger.Client.Constants.ErrorMessages;

namespace WX.B2C.User.Verification.LexisNexis.Bridger.Client
{
    internal static class ServiceClientExtensions
    {
        public static async Task<TResponse> ProcessRequestAsync<TClient, TResponse>(
            this TClient client,
            Func<TClient, Task<TResponse>> func) where TClient : ICommunicationObject
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            try
            {
                var response = await WrapWithRetryPolicy(() => func(client));
                client.Close();
                return response;
            }
            catch (Exception exc)
            {
                client.Abort();
                throw GetBridgerException(exc);
            }
            finally
            {
                ((IDisposable) client).Dispose();
            }
        }

        private static BridgerException GetBridgerException(Exception exc)
        {
            return exc switch
            {
                FaultException<ServiceFault> faultException => new BridgerException(faultException.Detail?.Type.ToString() ?? Unknown, faultException.Message, faultException),
                ServerTooBusyException tooBusyException => new BridgerUnavailableException(ServerTooBusy, tooBusyException),
                EndpointNotFoundException endpointNotFoundException => new BridgerUnavailableException(EndpointNotFound, endpointNotFoundException),
                CommunicationException communicationException => new BridgerUnavailableException(CommunicationError, communicationException),
                TimeoutException timeoutException => new BridgerUnavailableException(Timeout, timeoutException),
                _ => new BridgerException(exc.Message, exc)
            };
        }

        private static Task<TResponse> WrapWithRetryPolicy<TResponse>(Func<Task<TResponse>> func)
        {
            var policy = Policy.Handle<ProtocolException>(exception => exception.Message.Contains("TooManyRequests"))
                               .WaitAndRetryAsync(5, i => TimeSpan.FromMilliseconds(100 * Math.Pow(i, 2)));

            return policy.ExecuteAsync(func);
        }
    }
}