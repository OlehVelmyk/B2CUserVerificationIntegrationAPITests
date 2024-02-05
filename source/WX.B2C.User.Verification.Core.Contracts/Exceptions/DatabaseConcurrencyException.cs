using System;
using System.Runtime.Serialization;

namespace WX.B2C.User.Verification.Core.Contracts.Exceptions
{
    /// <summary>
    /// ORM agnostic serializable exception in data access layer for implementing optimistic concurrency strategy
    /// </summary>
    [Serializable]
    public class DatabaseConcurrencyException : DatabaseException
    {
        public DatabaseConcurrencyException(Exception exception)
            : base(exception)
        {
        }

        protected DatabaseConcurrencyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}