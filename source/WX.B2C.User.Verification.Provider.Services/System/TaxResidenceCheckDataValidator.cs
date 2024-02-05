using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    internal sealed class TaxResidenceCheckDataValidator : BaseCheckInputValidator<TaxResidenceInputData>
    {
        public TaxResidenceCheckDataValidator(TaxResidenceCheckConfiguration configuration)
            : base(configuration) { }

        protected override IEnumerable<string> Validate(CheckInputData checkData, out TaxResidenceInputData validatedData)
        {
            var missingData = new List<string>();

            validatedData = new TaxResidenceInputData
            {
                TaxResidence = checkData.TryGetValue<string[]>(XPathes.TaxResidence, missingData)
            };

            return missingData;
        }
    }
}