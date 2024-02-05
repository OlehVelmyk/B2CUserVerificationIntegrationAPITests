using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using WX.B2C.User.Verification.Onfido.Client.Models;

namespace WX.B2C.User.Verification.Onfido
{
    internal abstract class BaseOnfidoGateway
    {
        protected readonly ILogger Logger;

        protected BaseOnfidoGateway(ILogger logger)
        {
            Logger = logger?.ForContext(GetType()) ?? throw new ArgumentNullException(nameof(logger));
        }

        internal Task<TResponse> HandleAsync<T1, T2, TResponse>(
            Func<(T1, T2)> requestFactory,
            Func<T1, T2, CancellationToken, Task<TResponse>> requestInvoker,
            [CallerMemberName] string callerMethod = null)
        {
            return HandleAsync(
                requestFactory,
                requestInvoker,
                response => response,
                callerMethod: callerMethod);
        }

        internal Task<TResult> HandleAsync<T1, T2, TResponse, TResult>(
            Func<(T1, T2)> requestFactory,
            Func<T1, T2, CancellationToken, Task<TResponse>> requestInvoker,
            Func<TResponse, TResult> responseMapper,
            [CallerMemberName] string callerMethod = null)
        {
            return HandleAsync(
                requestFactory,
                (tuple, token) => requestInvoker(tuple.Item1, tuple.Item2, token),
                responseMapper,
                callerMethod: callerMethod);
        }

        internal Task<TResult> HandleAsync<T1, T2, T3, TResponse, TResult>(
            Func<(T1, T2, T3)> requestFactory,
            Func<T1, T2, T3, CancellationToken, Task<TResponse>> requestInvoker,
            Func<TResponse, TResult> responseMapper,
            [CallerMemberName] string callerMethod = null)
        {
            return HandleAsync(
                requestFactory,
                (tuple, token) => requestInvoker(tuple.Item1, tuple.Item2, tuple.Item3, token),
                responseMapper,
                callerMethod: callerMethod);
        }

        internal Task<TResult> HandleAsync<TRequest, TResponse, TResult>(
            Func<TRequest> requestFactory,
            Func<TRequest, CancellationToken, Task<TResponse>> requestInvoker,
            Func<TResponse, TResult> responseMapper,
            CancellationToken cancellationToken = default,
            [CallerMemberName] string callerMethod = null)
        {
            return HandleAsync(
                requestFactory,
                request => requestInvoker(request, cancellationToken),
                responseMapper,
                responseValidator: null,
                callerMethod: callerMethod);
        }

        private async Task<TResult> HandleAsync<TRequest, TResponse, TResult>(
            Func<TRequest> requestFactory,
            Func<TRequest, Task<TResponse>> requestInvoker,
            Func<TResponse, TResult> responseMapper,
            Action<TResponse> responseValidator = null,
            [CallerMemberName] string callerMethod = null)
        {
            if (requestFactory == null)
                throw new ArgumentNullException(nameof(requestFactory));
            if (requestInvoker == null)
                throw new ArgumentNullException(nameof(requestInvoker));
            if (responseMapper == null)
                throw new ArgumentNullException(nameof(responseMapper));

            var providerName = GetType().Name;

            try
            {
                var request = requestFactory();

                Logger.Debug("{Class}.{Method} sending request to Onfido API {@Request}", providerName, callerMethod, request);

                var response = await requestInvoker.Invoke(request);

                Logger.Debug("{Class}.{Method} received response from Onfido API {@Response}", providerName, callerMethod, response);

                responseValidator?.Invoke(response);

                return responseMapper(response);
            }
            catch (Exception exc)
            {
                if (exc is OnfidoApiErrorException apiError)
                    SanitizeForLogs(apiError);

                Logger.Error(
                    exc,
                    "{Class}.{Method} got error while processing response from Onfido {@ErrorMessage}",
                    providerName,
                    callerMethod,
                    exc.Message);

                throw new OnfidoApiException(exc.Message);
            }
        }

        private static OnfidoApiErrorException SanitizeForLogs(OnfidoApiErrorException exception)
        {
            exception.Request?.Headers?.Clear();
            return exception;
        }
    }
}
