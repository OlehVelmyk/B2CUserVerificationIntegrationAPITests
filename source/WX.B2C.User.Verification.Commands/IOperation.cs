using System;

namespace WX.B2C.User.Verification.Commands
{

    /// <summary>
    /// TODO later can be used as universal marker for all contexts
    /// </summary>
    public interface IOperation
    {
        Guid CorrelationId { get; }

        Guid OperationId { get; }
    }
}