using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.IoC;

namespace WX.B2C.User.Verification.Provider.Services
{
    public interface ICheckProviderRegistry
    {
        ICheckProviderFactory GetFactory(CheckProviderType providerType, CheckType? checkType = null);
    }

    internal sealed class AutofacCheckProviderRegistry : ICheckProviderRegistry
    {
        private readonly IEnumerable<Lazy<ICheckProviderFactory, CheckProviderMetadata>> _checkProviderFactories;

        public AutofacCheckProviderRegistry(
            IEnumerable<Lazy<ICheckProviderFactory, CheckProviderMetadata>> checkProviderFactories)
        {
            _checkProviderFactories = checkProviderFactories ?? throw new ArgumentNullException(nameof(checkProviderFactories));
        }

        public ICheckProviderFactory GetFactory(CheckProviderType providerType, CheckType? checkType = null)
        {
            var checkProviderFactory = _checkProviderFactories.FirstOrDefault(s =>
                s.Metadata.ProviderType == providerType && s.Metadata.CheckType == checkType);

            if (checkProviderFactory == null)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(checkProviderFactory),
                    $"{providerType}:{checkType}",
                    "Unsupported check provider factory.");
            }

            return checkProviderFactory.Value;
        }
    }
}