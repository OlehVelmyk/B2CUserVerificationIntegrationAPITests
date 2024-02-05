using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;

namespace WX.B2C.User.Verification.DataAccess
{
    internal class SqlServerRetryingExecutionStrategy : Microsoft.EntityFrameworkCore.SqlServerRetryingExecutionStrategy
    {
        private readonly ILogger _logger;

        /// <summary>
        ///     Method called before retrying the operation execution
        /// </summary>
        protected override void OnRetry()
        {
            var lastException = ExceptionsEncountered.LastOrDefault();
            if (lastException != null)
                _logger.Information(lastException, "Transient database error occurred. Retrying.");
        }

        public SqlServerRetryingExecutionStrategy(ILogger logger, ExecutionStrategyDependencies dependencies)
            : this(logger, dependencies, DefaultMaxRetryCount)
        {
        }

        public SqlServerRetryingExecutionStrategy(ILogger logger, ExecutionStrategyDependencies dependencies, int maxRetryCount)
            : this(logger, dependencies, maxRetryCount, DefaultMaxDelay, null)
        {
        }

        public SqlServerRetryingExecutionStrategy(ILogger logger, ExecutionStrategyDependencies dependencies, int maxRetryCount, TimeSpan maxRetryDelay, ICollection<int> errorNumbersToAdd) 
            : base(dependencies, maxRetryCount, maxRetryDelay, errorNumbersToAdd)
        {
            _logger = logger?.ForContext<SqlServerRetryingExecutionStrategy>() ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}
