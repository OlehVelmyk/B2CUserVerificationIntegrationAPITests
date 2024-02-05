using System;
using Optional;
using WX.B2C.User.Verification.PassFort.Models;

namespace WX.B2C.User.Verification.PassFort.Mappers
{
    internal interface IProfileDataPatchMapper
    {
        PassFortProfilePatch Map(RiskScreeningCheckData checkInputData);
    }

    internal class ProfileDataPatchMapper : IProfileDataPatchMapper
    {
        public PassFortProfilePatch Map(RiskScreeningCheckData checkInputData)
        {
            if (checkInputData == null)
                throw new ArgumentNullException(nameof(checkInputData));

            return new PassFortProfilePatch
            {
                Address = checkInputData.ResidenceAddress.Some(),
                FullName = checkInputData.FullName.Some(),
                BirthDate = checkInputData.BirthDate.Some(),
                Nationality = checkInputData.Nationality.Some()
            };
        }
    }
}
