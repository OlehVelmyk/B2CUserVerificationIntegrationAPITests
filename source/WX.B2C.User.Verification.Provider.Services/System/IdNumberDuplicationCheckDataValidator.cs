using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    internal sealed class IdNumberDuplicationCheckDataValidator : BaseCheckInputValidator<IdNumberDuplicationInputData>
    {
        public IdNumberDuplicationCheckDataValidator(IdNumberDuplicationCheckConfiguration configuration)
            : base(configuration) { }

        protected override IEnumerable<string> Validate(CheckInputData checkData, out IdNumberDuplicationInputData validatedData)
        {
            var missingData = new List<string>();

            validatedData = new IdNumberDuplicationInputData
            {
                UserId = checkData.UserId,
                IdDocumentNumber = checkData.TryGetValue<IdDocumentNumberDto>(XPathes.IdDocumentNumber, missingData)
            };

            return missingData;
        }
    }
}