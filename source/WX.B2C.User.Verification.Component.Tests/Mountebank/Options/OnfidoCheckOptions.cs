using System;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Component.Tests.Mountebank.Options
{
    internal abstract class BaseOnfidoCheckOptions
    {
        public string ApplicantId { get; protected set; }
    }

    internal class OnfidoCheckOptions : BaseOnfidoCheckOptions
    {
        public OnfidoCheckOption Check { get; set; }

        private OnfidoCheckOptions() { }

        public static OnfidoCheckOptions Create(string applicantId, OnfidoCheckOption check) =>
            new()
            {
                ApplicantId = applicantId,
                Check = check ?? throw new ArgumentNullException(nameof(check))
            };
    }

    internal class OnfidoGroupedCheckOptions : BaseOnfidoCheckOptions
    {
        public OnfidoCheckOption[] Checks { get; private set; }

        private OnfidoGroupedCheckOptions() { }

        public static OnfidoGroupedCheckOptions Create(string applicantId, params OnfidoCheckOption[] checks)
        {
            if (checks.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(checks));

            return new()
            {
                ApplicantId = applicantId,
                Checks = checks
            };
        }
    }

    internal class OnfidoCheckOption
    {
        public CheckType Type { get; private set; }

        public CheckResult Result { get; private set; }

        public string Decision { get; private set; }

        public string PoiIssuingCountry { get; private set; } = Alpha3CountryCodes.Gb;

        private OnfidoCheckOption() { }

        public OnfidoCheckOption WithPoiIssuingCountry(string alfa3)
        {
            PoiIssuingCountry = alfa3 ?? throw new ArgumentNullException(nameof(alfa3));
            return this;
        }

        public static OnfidoCheckOption Passed(CheckType type) =>
            new()
            {
                Type = type,
                Result = CheckResult.Passed
            };

        public static OnfidoCheckOption Failed(CheckType type, string decision = null) =>
            new()
            {
                Type = type,
                Result = CheckResult.Failed,
                Decision = decision
            };
    }
}
