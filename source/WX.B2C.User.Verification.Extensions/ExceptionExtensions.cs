using System;
using System.Collections.Generic;
using System.Linq;

namespace WX.B2C.User.Verification.Extensions
{
    public static class ExceptionExtensions
    {
        public static IEnumerable<TException> InnerExceptionsOfType<TException>(this AggregateException exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            return exception.Flatten()
                            .InnerExceptions
                            .OfType<TException>();
        }
    }
}