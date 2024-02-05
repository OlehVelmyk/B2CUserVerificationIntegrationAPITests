using System;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Infrastructure.Common
{
    internal class IdempotentGuidGenerator : IIdempotentGuidGenerator
    {
        private readonly IOperationContextProvider _contextProvider;

        public IdempotentGuidGenerator(IOperationContextProvider contextProvider)
        {
            _contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        }

        public Guid Generate(int bias)
        {
            var context = _contextProvider.GetContextOrDefault();
            var baseSeed = context.CorrelationId.GetHashCode();
            long seed = baseSeed + bias;
            var random = new Random(seed.GetHashCode());
            var bytes = new byte[16];
            random.NextBytes(bytes);
            return new Guid(bytes);
        }
    }
}