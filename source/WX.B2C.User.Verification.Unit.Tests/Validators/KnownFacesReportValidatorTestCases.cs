using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using WX.B2C.User.Verification.Onfido.Client.Models;

namespace WX.B2C.User.Verification.Unit.Tests.Validators
{
    internal class KnownFacesReportValidatorTestCases : BaseReportValidatorTestCases
    {
        public static IEnumerable<Report> ClearTestCases =>
            GetClearReports.Value.Select(ToReport);

        public static IEnumerable<Report> FraudTestCases =>
            GetFraudReports.Value.Select(ToReport);

        public static IEnumerable<Report> ResubmitTestCases =>
            GetResubmitReports.Value.Select(ToReport);

        private static Lazy<JArray> GetClearReports => new(ReadReportsFromJson("clear"));

        private static Lazy<JArray> GetFraudReports => new(ReadReportsFromJson("fraud"));

        private static Lazy<JArray> GetResubmitReports => new(ReadReportsFromJson("resubmit"));

        private static JArray ReadReportsFromJson(string testCase) =>
            ReadReportsFromJson("known-faces-reports.json", testCase);
    }
}
