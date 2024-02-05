using System;

namespace WX.B2C.User.Verification.Infrastructure.Contracts
{
    public interface IIdempotentGuidGenerator
    {
        Guid Generate(int bias);
    }
}
