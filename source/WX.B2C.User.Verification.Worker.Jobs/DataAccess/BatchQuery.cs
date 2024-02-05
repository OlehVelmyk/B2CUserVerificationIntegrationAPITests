using System;
using System.Collections.Generic;
using Dapper;

namespace WX.B2C.User.Verification.Worker.Jobs.DataAccess
{
    internal class BatchQuery
    {
        internal const string OffsetParameter = "@offset";
        internal const string BatchSizeParameter = "@batchSize";

        private readonly int _batchSize;
        private readonly CommandDefinition _commandDefinition;
        private readonly Dictionary<string, object> _parameters;

        public BatchQuery(string sql, int batchSize)
        {
            if (batchSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(batchSize), batchSize, "Batch size must be positive value.");

            _batchSize = batchSize;
            _parameters = new Dictionary<string, object>()
            {
                { BatchSizeParameter, batchSize }
            };
            _commandDefinition = new CommandDefinition(sql, _parameters);
        }

        public CommandDefinition ForPage(int page)
        {
            _parameters[OffsetParameter] = page * _batchSize;
            return _commandDefinition;
        }
    }
}