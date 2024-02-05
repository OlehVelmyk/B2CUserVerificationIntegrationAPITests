using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using WX.B2C.User.Verification.Onfido.Client.Models;

namespace WX.B2C.User.Verification.Unit.Tests.Validators
{
    internal class DocumentReportValidatorTestCases : BaseReportValidatorTestCases
    {
        public static IEnumerable<DocumentReport> ClearTestCases =>
            GetClearReports.Value
                .Concat(ExtraClearTestCases())
                .Select(ToReport<DocumentReport>);

        public static IEnumerable<DocumentReport> ResubmitTestCases =>
            GetResubmitReports.Value
                .Concat(ExtraResubmitTestCases())
                .Select(ToReport<DocumentReport>);

        public static IEnumerable<DocumentReport> FraudTestCases =>
            GetFraudReports.Value
                .Concat(ExtraFraudTestCases())
                .Select(ToReport<DocumentReport>);

        private static Lazy<JArray> GetClearReports => new(ReadReportsFromJson("clear"));

        private static Lazy<JArray> GetResubmitReports => new(ReadReportsFromJson("resubmit"));

        private static Lazy<JArray> GetFraudReports => new(ReadReportsFromJson("fraud"));

        private static IEnumerable<JToken> ExtraClearTestCases()
        {
            var report = GetClearReports.Value.First();

            // DataConsistency
            yield return report.OverrideDataConsistencyResult("gender");
            yield return report.OverrideDataConsistencyResult("issuing_country");
            yield return report.OverrideDataConsistencyResult("nationality");
            yield return report.OverrideDataConsistencyResult("document_numbers");
            yield return report.OverrideDataConsistencyResult("multiple_data_sources_present");

            // DataComparison
            yield return report.OverrideDataComparisonResult("date_of_expiry");
            yield return report.OverrideDataComparisonResult("document_numbers");
            yield return report.OverrideDataComparisonResult("gender");
            yield return report.OverrideDataComparisonResult("issuing_country");

            // DataValidation
            yield return report.OverrideDataValidationResult("document_numbers");
            yield return report.OverrideDataValidationResult("gender");
            yield return report.OverrideDataValidationResult("barcode");

            // AgeValidation
            yield return report.OverrideBreakdownResult("age_validation", "minimum_accepted_age");

            // AgeValidation
            yield return report.OverrideBreakdownResult("police_record");

            // Complex
            yield return report
                         .OverrideDataConsistencyResult("gender")
                         .OverrideDataComparisonResult("document_numbers")
                         .OverrideDataValidationResult("barcode")
                         .OverrideBreakdownResult("police_record");
        }

        private static IEnumerable<JToken> ExtraResubmitTestCases()
        {
            var report = GetClearReports.Value.First();

            // VisualAuthenticity
            yield return report.OverrideVisualAuthenticityResult("other");
            yield return report.OverrideVisualAuthenticityResult("picture_face_integrity");
            yield return report.OverrideVisualAuthenticityResult("original_document_present");

            // DataComparison
            yield return report.OverrideDataComparisonResult("date_of_birth");
            yield return report.OverrideDataComparisonResult("first_name");
            yield return report.OverrideDataComparisonResult("last_name");
            yield return report.OverrideDataComparisonResult("document_type");

            // DataConsistency
            yield return report.OverrideDataConsistencyResult("document_type");
            yield return report.OverrideDataConsistencyResult("date_of_expiry");
            yield return report.OverrideDataConsistencyResult("date_of_birth");
            yield return report.OverrideDataConsistencyResult("first_name");
            yield return report.OverrideDataConsistencyResult("last_name");

            // DataValidation
            yield return report.OverrideDataValidationResult("date_of_birth");
            yield return report.OverrideDataValidationResult("document_expiration");
            yield return report.OverrideDataValidationResult("expiry_date");
            yield return report.OverrideDataValidationResult("mrz");


            // ImageIntegrity
            yield return report.OverrideImageIntegrityResult("image_quality");
            yield return report.OverrideImageIntegrityResult("conclusive_document_quality");
            yield return report.OverrideImageIntegrityResult("supported_document");
            yield return report.OverrideImageIntegrityResult("colour_picture");

            // CompromisedDocument
            yield return report.OverrideBreakdownResult("compromised_document");

            // Complex
            yield return report
                         .OverrideVisualAuthenticityResult("original_document_present")
                         .OverrideDataComparisonResult("date_of_birth")
                         .OverrideDataConsistencyResult("last_name")
                         .OverrideDataValidationResult("document_expiration")
                         .OverrideImageIntegrityResult("image_quality")
                         .OverrideBreakdownResult("compromised_document");
        }

        private static IEnumerable<JToken> ExtraFraudTestCases()
        {
            var report = GetClearReports.Value.First();

            // VisualAuthenticity
            yield return report.OverrideVisualAuthenticityResult("fonts");
            yield return report.OverrideVisualAuthenticityResult("template");
            yield return report.OverrideVisualAuthenticityResult("face_detection");
            yield return report.OverrideVisualAuthenticityResult("security_features");
            yield return report.OverrideVisualAuthenticityResult("digital_tampering");

            // Complex
            yield return report
                         .OverrideVisualAuthenticityResult("fonts")
                         .OverrideDataComparisonResult("template");
        }

        private static JArray ReadReportsFromJson(string testCase) =>
            ReadReportsFromJson("document-reports.json", testCase);
    }
}
