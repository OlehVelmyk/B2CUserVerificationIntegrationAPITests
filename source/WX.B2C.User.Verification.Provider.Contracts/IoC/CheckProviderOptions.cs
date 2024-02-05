using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Provider.Contracts.Stubs;

namespace WX.B2C.User.Verification.Provider.Contracts.IoC
{
    public class CheckProviderOptions
    {
        public CheckProviderOptions(CheckProviderType providerType)
        {
            ProviderType = providerType;
            Factories = new Dictionary<CheckType, Type>();
            Configurations = new Dictionary<CheckType, Type>();
            StubFactories = new Dictionary<CheckType, ICheckProcessingResultFactory>();
        }

        public CheckProviderType ProviderType { get; }

        public IDictionary<CheckType, Type> Factories { get; }

        public Type ComplexFactory { get; private set; }
        
        public IDictionary<CheckType, ICheckProcessingResultFactory> StubFactories{ get; }

        public IDictionary<CheckType, Type> Configurations { get; }

        public void AddFactory<TFactory>(CheckType checkType, ICheckProcessingResultFactory checkResultStub = null) where TFactory: ICheckProviderFactory
        {
            var factoryType = typeof(TFactory);
            Factories.Add(checkType, factoryType);
            
            if (checkResultStub != null)
                StubFactories.Add(checkType, checkResultStub);

            var configurationType = GetConfigurationType(factoryType);
            Configurations.Add(checkType, configurationType);
        }

        public void AddComplexFactory<TFactory>()
        {
            ComplexFactory = typeof(TFactory);
        }

        private static Type GetConfigurationType(Type type)
        {
            while (type.BaseType != null)
            {
                type = type.BaseType;
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(BaseCheckProviderFactory<>))
                    return type.GetGenericArguments()[0];
            }

            throw new InvalidOperationException("Base type was not found.");
        }
    }
}