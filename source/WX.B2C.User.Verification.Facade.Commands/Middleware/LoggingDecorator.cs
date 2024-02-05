using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using WX.Commands;

namespace WX.B2C.User.Verification.Facade.Commands.Middleware
{
    internal class LoggingDecorator : ICommandHandlerDecorator
    {
        private readonly LogEventLevel _logLevel;
        private readonly ILogger _logger;

        public LoggingDecorator(ILogger logger, LogEventLevel logLevel = LogEventLevel.Information)
        {
            _logLevel = logLevel;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleAsync<T>(T command, Func<T, Task> processor) where T : Command
        {
            var stopwatch = new Stopwatch();
            var logger = _logger.ForContext("CommandType", command.GetType().Name)
                                .ForContext("Command", command, true);
            try
            {
                logger.Write(_logLevel, "Started processing command");

                stopwatch.Start();
                await processor(command);
                stopwatch.Stop();

                logger.Write(_logLevel,
                             "Finished processing command, elapsed: {Elapsed}",
                             stopwatch.Elapsed.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to process command, elapsed: {Elapsed}", 
                             stopwatch.Elapsed.TotalMilliseconds);
                throw;
            }
            finally
            {
                stopwatch.Stop();
            }
        }
    }
}