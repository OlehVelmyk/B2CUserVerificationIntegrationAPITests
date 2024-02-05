using System;
using WX.B2C.User.Verification.Provider.Contracts.Models;
using WX.B2C.User.Verification.Provider.Contracts.Stubs;

namespace WX.B2C.User.Verification.Provider.Contracts
{
    public class CheckProviderStubFactory : ICheckProviderFactory
    {
        private readonly ICheckProcessingResultFactory _checkProcessingResultFactory;

        public CheckProviderStubFactory(ICheckProcessingResultFactory checkProcessingResultFactory)
        {
            _checkProcessingResultFactory = checkProcessingResultFactory ?? throw new ArgumentNullException(nameof(checkProcessingResultFactory));
        }

        public CheckProvider Create(CheckProviderConfiguration configuration) => new CheckProviderStub(_checkProcessingResultFactory);
    }
}