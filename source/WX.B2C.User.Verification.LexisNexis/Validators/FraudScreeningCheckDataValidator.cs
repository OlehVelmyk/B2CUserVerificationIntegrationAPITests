using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.LexisNexis.Models;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.LexisNexis.Validators
{
    internal sealed class FraudScreeningCheckDataValidator : BaseCheckInputValidator<FraudScreeningCheckData>
    {
        public FraudScreeningCheckDataValidator(CheckProviderConfiguration configuration)
            : base(configuration) { }

        protected override IEnumerable<string> Validate(CheckInputData inputData, out FraudScreeningCheckData validatedData)
        {
            var missingData = new List<string>();

            validatedData = new FraudScreeningCheckData
            {
                FullName = inputData.TryGetValue<FullNameDto>(XPathes.FullName, missingData),
                BirthDate = inputData.TryGetValue<DateTime>(XPathes.Birthdate, missingData),
                Address = inputData.TryGetValue<AddressDto>(XPathes.ResidenceAddress, missingData),
                Tin = inputData.TryGetValue<TinDto>(XPathes.Tin, missingData),
            };

            return missingData;
        }
    }
}