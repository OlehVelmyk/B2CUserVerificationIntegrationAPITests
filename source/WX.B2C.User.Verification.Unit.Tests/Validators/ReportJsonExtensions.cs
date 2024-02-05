using System;
using Newtonsoft.Json.Linq;
using WX.B2C.User.Verification.Onfido.Client.Models;

namespace WX.B2C.User.Verification.Unit.Tests.Validators
{
    internal static class ReportJsonExtensions
    {
        public static JToken OverrideDataConsistencyResult(this JToken report, string breakdownName) =>
            report.OverrideBreakdownResult("data_consistency", breakdownName);

        public static JToken OverrideDataComparisonResult(this JToken report, string breakdownName) =>
            report.OverrideBreakdownResult("data_comparison", breakdownName);

        public static JToken OverrideVisualAuthenticityResult(this JToken report, string breakdownName) =>
            report.OverrideBreakdownResult("visual_authenticity", breakdownName);

        public static JToken OverrideDataValidationResult(this JToken report, string breakdownName) =>
            report.OverrideBreakdownResult("data_validation", breakdownName);

        public static JToken OverrideImageIntegrityResult(this JToken report, string breakdownName) =>
            report.OverrideBreakdownResult("image_integrity", breakdownName);

        public static JToken OverrideBreakdownResult(this JToken report, params string[] breakdownNames)
        {
            var breakdownResult = ReportSubResult.Caution.ToString();

            if (report == null)
                throw new ArgumentNullException(nameof(report));
            if (breakdownNames == null)
                throw new ArgumentNullException(nameof(breakdownNames));

            var clone = report.DeepClone();

            var breakdown = clone;
            foreach (var breakdownName in breakdownNames)
            {
                breakdown = breakdown.SelectToken($"breakdown.{breakdownName}") ?? JToken.Parse("{}");
                breakdown["result"] = breakdownResult;
            }

            return clone;
        }
    }
}