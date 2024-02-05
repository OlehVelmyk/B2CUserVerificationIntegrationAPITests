using System;
using WX.B2C.User.Verification.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Component.Tests.Mountebank.Options
{
    internal class PassfortCheckOptions
    {
        public string ProfileId { get; private set; }

        public CheckResult Result { get; private set; }

        public bool IsSanctioned { get; private set; }

        public bool IsPep { get; private set; }

        public bool IsAdverseMedia { get; private set; }

        private PassfortCheckOptions() { }

        public PassfortCheckOptions WithProfileId(string profileId)
        {
            ProfileId = profileId ?? throw new ArgumentNullException(nameof(profileId));
            return this;
        }

        public static PassfortCheckOptions Passed() =>
            new()
            {
                Result = CheckResult.Passed
            };

        public static PassfortCheckOptions Failed(bool isSanctioned = false, bool isPep = false, bool isAdverseMedia = false) =>
            new()
            {
                Result = CheckResult.Failed,
                IsSanctioned = isSanctioned,
                IsPep = isPep,
                IsAdverseMedia = isAdverseMedia
            };
    }
}
