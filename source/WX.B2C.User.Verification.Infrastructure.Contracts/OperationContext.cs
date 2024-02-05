using System;

namespace WX.B2C.User.Verification.Infrastructure.Contracts
{
    public class OperationContext
    {
        public static readonly OperationContext Default =
            new OperationContext
            {
                CorrelationId = Guid.Empty,
                OperationId = Guid.Empty,
                ParentOperationId = null,
                OperationName = null
            };

        private OperationContext()
        {
        }

        public Guid CorrelationId { get; private set; }

        public Guid? ParentOperationId { get; private set; }

        public Guid OperationId { get; private set; }

        public string OperationName { get; private set; }

        public static OperationContext Create(
            Guid correlationId,
            Guid? parentOperationId,
            Guid operationId,
            string operationName)
        {
            return new OperationContext
            {
                CorrelationId = correlationId,
                OperationId = operationId,
                ParentOperationId = parentOperationId,
                OperationName = operationName
            };
        }
    }
}
