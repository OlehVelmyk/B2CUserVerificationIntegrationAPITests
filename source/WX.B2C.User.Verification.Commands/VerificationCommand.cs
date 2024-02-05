using System;
using WX.Commands;

namespace WX.B2C.User.Verification.Commands
{
    public abstract class VerificationCommand : Command, IOperation
    {
        public Guid CorrelationId { get; set; }

        public Guid OperationId { get; set; }
    }
}