using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;
using Serilog;
using WX.Commands;

namespace WX.B2C.User.Verification.Listener.Commands
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class CommandListenerService : StatelessService
    {
        private readonly ICommandsProcessor _commandsProcessor;
        private readonly ILogger _logger;

        public CommandListenerService(StatelessServiceContext context, ICommandsProcessor commandsProcessor, ILogger logger)
            : base(context)
        {
            _commandsProcessor = commandsProcessor ?? throw new ArgumentNullException(nameof(commandsProcessor));
            _logger = logger?.ForContext<CommandListenerService>() ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _commandsProcessor.ProcessCommandsAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.Information("Cancellation requested. RunAsync completed gracefully");
                throw;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error in RunAsync");
                throw;
            }
        }
    }
}
