using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
using Serilog;
using WX.B2C.User.Verification.PassFort.Exceptions;

namespace WX.B2C.User.Verification.PassFort
{
    internal abstract class BasePassFortGateway
    {
        protected ILogger Logger { get; }

        protected BasePassFortGateway(ILogger logger)
        {
            Logger = logger?.ForContext(GetType()) ?? throw new ArgumentNullException(nameof(logger));
        }

        protected Task<TResponse> HandleAsync<T1, T2, TResponse>(
            Func<(T1, T2)> requestFactory,
            Func<T1, T2, CancellationToken, Task<TResponse>> requestInvoker,
            CancellationToken cancellationToken = default,
            [CallerMemberName] string callerMethod = null) =>
            HandleAsync(
                requestFactory,
                (tuple, token) => requestInvoker(tuple.Item1, tuple.Item2, token),
                response => response,
                cancellationToken,
                callerMethod);

        protected Task<TResponse> HandleAsync<T1, T2, T3, TResponse>(
            Func<(T1, T2, T3)> requestFactory,
            Func<T1, T2, T3, CancellationToken, Task<TResponse>> requestInvoker,
            CancellationToken cancellationToken = default,
            [CallerMemberName] string callerMethod = null) =>
            HandleAsync(
                requestFactory,
                (tuple, token) => requestInvoker(tuple.Item1, tuple.Item2, tuple.Item3, token),
                response => response,
                cancellationToken,
                callerMethod);

        protected Task<TResult> HandleAsync<T1, T2, TResponse, TResult>(
            Func<(T1, T2)> requestFactory,
            Func<T1, T2, CancellationToken, Task<TResponse>> requestInvoker,
            Func<TResponse, TResult> responseMapper,
            CancellationToken cancellationToken = default,
            [CallerMemberName] string callerMethod = null) =>
            HandleAsync(
                requestFactory,
                (tuple, token) => requestInvoker(tuple.Item1, tuple.Item2, token),
                responseMapper,
                cancellationToken,
                callerMethod);

        protected Task<TResult> HandleAsync<TRequest, TResponse, TResult>(
            Func<TRequest> requestFactory,
            Func<TRequest, CancellationToken, Task<TResponse>> requestInvoker,
            Func<TResponse, TResult> responseMapper,
            CancellationToken cancellationToken = default,
            [CallerMemberName] string callerMethod = null) =>
            HandleAsync(
                requestFactory,
                request => requestInvoker(request, cancellationToken),
                responseMapper,
                callerMethod);

        protected async Task<TResult> HandleAsync<TRequest, TResponse, TResult>(
            Func<TRequest> requestFactory,
            Func<TRequest, Task<TResponse>> requestInvoker,
            Func<TResponse, TResult> responseMapper,
            [CallerMemberName] string callerMethod = null)
        {
            if (requestFactory == null)
                throw new ArgumentNullException(nameof(requestFactory));
            if (requestInvoker == null)
                throw new ArgumentNullException(nameof(requestInvoker));
            if (responseMapper == null)
                throw new ArgumentNullException(nameof(responseMapper));

            var className = GetType().Name;

            try
            {
                var request = requestFactory();

                Logger.Debug("{Class}.{Method} sending request to PassFort API {@Request}", className, callerMethod, request);

                var response = await requestInvoker.Invoke(request);

                Logger.Debug("{Class}.{Method} received response from PassFort API {@Response}", className, callerMethod, response);

                return responseMapper(response);
            }
            catch (Exception exc)
            {
                if (exc is HttpOperationException httpException)
                    SanitizeForLogs(httpException);

                Logger.Error(
                    exc,
                    "{Class}.{Method} got error while processing response from PassFort API {@ErrorMessage}",
                    className,
                    callerMethod,
                    exc.Message);

                throw new PassFortApiException(exc.Message);
            }
        }


        private static HttpOperationException SanitizeForLogs(HttpOperationException exception)
        {
            exception.Request?.Headers?.Clear();
            return exception;
        }
    }
}