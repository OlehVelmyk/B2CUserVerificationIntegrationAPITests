using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Serilog;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Facade.Controllers.Webhook.Handlers
{
    /// <summary>
    /// Handle request in background 
    /// </summary>
    public abstract class ForgettableHandler<TRequest> : IRequestHandler<TRequest>
        where TRequest : IRequest
    {
        protected readonly ILogger _logger;

        protected ForgettableHandler(ILogger logger)
        {
            _logger = logger?.ForContext(GetType()) ?? throw new ArgumentNullException(nameof(logger));
        }

        Task<Unit> IRequestHandler<TRequest, Unit>.Handle(TRequest request, CancellationToken cancellationToken)
        {
            Handle(request).RunAndForget(exc => HandleException(request, exc));
            return Unit.Task;
        }

        protected abstract Task Handle(TRequest request);

        private void HandleException(TRequest request, Exception exception) =>
            _logger
                .ForContext(nameof(request), request)
                .Error(exception,
                    "An error occurred while processing {RequestType} request in background task.",
                    request.GetType().FullName);
    }
}
