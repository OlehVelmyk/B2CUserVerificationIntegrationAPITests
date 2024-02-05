using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Provider.Contracts.Exceptions;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Contracts
{
    public abstract class BaseCheckInputValidator<TData>
    {
        private readonly CheckProviderConfiguration _configuration;

        protected BaseCheckInputValidator(CheckProviderConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public TData Validate(CheckInputData inputData)
        {
            var optionalData = _configuration.CheckParameters
                                             .Where(parameter => !parameter.IsRequired)
                                             .Select(parameter => parameter.XPath)
                                             .ToArray();

            var missingData = Validate(inputData, out var result).ToArray();
            var missingRequiredData = missingData.Except(optionalData);
            if (missingRequiredData.Any())
                throw new CheckInputValidationException(missingData);

            return result;
        }

        protected abstract IEnumerable<string> Validate(CheckInputData inputData, out TData validatedData);
    }
}