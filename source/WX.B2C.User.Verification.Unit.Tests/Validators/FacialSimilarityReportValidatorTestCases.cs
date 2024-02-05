using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using WX.B2C.User.Verification.Onfido.Client.Models;

namespace WX.B2C.User.Verification.Unit.Tests.Validators
{
    internal class FacialSimilarityReportValidatorTestCases : BaseReportValidatorTestCases
    {
        public static IEnumerable<Report> ClearTestCases =>
            GetClearReports.Value.Select(ToReport);

        public static IEnumerable<Report> ResubmitTestCases =>
            GetResubmitReports.Value
                              .Concat(ExtraResubmitTestCases())
                              .Select(ToReport);

        public static IEnumerable<Report> ConsiderTestCases =>
            GetConsiderReports.Value
                              .Select(ToReport);

        private static Lazy<JArray> GetClearReports => new(ReadReportsFromJson("clear"));

        private static Lazy<JArray> GetResubmitReports => new(ReadReportsFromJson("resubmit"));

        private static Lazy<JArray> GetConsiderReports => new(ReadReportsFromJson("consider"));

        private static IEnumerable<JToken> ExtraResubmitTestCases()
        {
            var report = GetClearReports.Value.First();

            // ImageIntegrity
            yield return report.OverrideBreakdownResult("image_integrity", "source_integrity");
            yield return report.OverrideBreakdownResult("image_integrity", "face_detected");

            // VisualAuthenticity
            yield return report.OverrideBreakdownResult("visual_authenticity");
            yield return report.OverrideBreakdownResult("visual_authenticity", "spoofing_detection");
            yield return report.OverrideBreakdownResult("visual_authenticity", "liveness_detected");

            // FaceComparison
            yield return report.OverrideBreakdownResult("face_comparison");
            yield return report.OverrideBreakdownResult("face_comparison", "face_match");

            // Complex
            yield return report
                         .OverrideBreakdownResult("image_integrity", "source_integrity")
                         .OverrideBreakdownResult("visual_authenticity", "liveness_detected");
        }

        private static JArray ReadReportsFromJson(string testCase) =>
            ReadReportsFromJson("facial-similarity-reports.json", testCase);
    }
}
