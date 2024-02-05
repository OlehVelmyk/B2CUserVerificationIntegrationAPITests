using System;
using System.Runtime.Serialization;
using WX.B2C.User.Verification.Domain.Exceptions;

namespace WX.B2C.User.Verification.Core.Contracts.Exceptions
{
    /// <summary>
    /// ORM agnostic serializable exception for any exceptions in data access layer.
    /// </summary>
    [Serializable]
    public class DatabaseException : B2CVerificationException
    {
        public DatabaseException(Exception exception) :
            base(exception.InnerException?.Message ?? exception.Message) { }

        protected DatabaseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}