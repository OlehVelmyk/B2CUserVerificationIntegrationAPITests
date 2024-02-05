using System;
using System.Threading;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Infrastructure.Remoting
{
    internal class OperationContextProvider : IOperationContextProvider, IOperationContextSetter
    {
        private static readonly AsyncLocal<OperationContext> Context = new AsyncLocal<OperationContext>();

        public void SetContext(OperationContext context)
        {
            Context.Value = context ?? throw new ArgumentNullException(nameof(context));
        }

        public OperationContext GetContextOrDefault()
        {
            return Context.Value ?? OperationContext.Default;
        }
    }

    internal interface IOperationContextSetter
    {
        public void SetContext(OperationContext context);
    }
}
