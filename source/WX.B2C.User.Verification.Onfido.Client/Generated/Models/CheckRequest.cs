// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Onfido.Client.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class CheckRequest
    {
        /// <summary>
        /// Initializes a new instance of the CheckRequest class.
        /// </summary>
        public CheckRequest()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the CheckRequest class.
        /// </summary>
        /// <param name="applicantId">The ID of the applicant to do the check
        /// on.</param>
        /// <param name="reportNames">An array of report names
        /// (strings).</param>
        /// <param name="privacyNoticesReadConsentGiven">Indicates whether the
        /// privacy notices and terms of service have been read and, where
        /// specific laws require, that consent has been given for
        /// Onfido.</param>
        /// <param name="tags">Array of tags being assigned to this
        /// check.</param>
        /// <param name="applicantProvidesData">Send an applicant form to
        /// applicant to complete to proceed with check. Defaults to false.
        /// </param>
        /// <param name="suppressFormEmails">For checks where
        /// `applicant_provides_data` is `true`, applicant form will not be
        /// automatically sent if `suppress_form_emails` is set to `true`. You
        /// can manually send the form at any time after the check has been
        /// created, using the link found in the form_uri attribute of the
        /// check object. Write-only. Defaults to false.
        /// </param>
        /// <param name="asynchronous">Defaults to `true`. Write-only. If set
        /// to `false`, you will only receive a response when all reports in
        /// your check have completed.
        /// </param>
        /// <param name="webhookIds">Optional. An array of strings describing
        /// which webhooks to trigger for this check. By default, all webhooks
        /// registered in the account will be triggered and this value will be
        /// null in the responses.</param>
        /// <param name="documentIds">Optional. Array of strings describing
        /// which document to process in checks containing a Document report or
        /// a Facial Similarity report, or both. By default, the most recently
        /// uploaded document is used. `document_ids` is only usable with
        /// Document and Facial Similarity reports.</param>
        /// <param name="redirectUri">For checks where
        /// `applicant_provides_data` is `true`, redirect to this URI when the
        /// applicant has submitted their data. Read-only.</param>
        /// <param name="consider">Returns a pre-determined consider sub-result
        /// in sandbox for the specific reports in the consider array.</param>
        public CheckRequest(string applicantId, IList<string> reportNames, bool? privacyNoticesReadConsentGiven = default(bool?), IList<string> tags = default(IList<string>), bool? applicantProvidesData = default(bool?), bool? suppressFormEmails = default(bool?), bool? asynchronous = default(bool?), IList<string> webhookIds = default(IList<string>), IList<string> documentIds = default(IList<string>), string redirectUri = default(string), IList<string> consider = default(IList<string>))
        {
            ApplicantId = applicantId;
            ReportNames = reportNames;
            PrivacyNoticesReadConsentGiven = privacyNoticesReadConsentGiven;
            Tags = tags;
            ApplicantProvidesData = applicantProvidesData;
            SuppressFormEmails = suppressFormEmails;
            Asynchronous = asynchronous;
            WebhookIds = webhookIds;
            DocumentIds = documentIds;
            RedirectUri = redirectUri;
            Consider = consider;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the ID of the applicant to do the check on.
        /// </summary>
        [JsonProperty(PropertyName = "applicant_id")]
        public string ApplicantId { get; set; }

        /// <summary>
        /// Gets or sets an array of report names (strings).
        /// </summary>
        [JsonProperty(PropertyName = "report_names")]
        public IList<string> ReportNames { get; set; }

        /// <summary>
        /// Gets or sets indicates whether the privacy notices and terms of
        /// service have been read and, where specific laws require, that
        /// consent has been given for Onfido.
        /// </summary>
        [JsonProperty(PropertyName = "privacy_notices_read_consent_given")]
        public bool? PrivacyNoticesReadConsentGiven { get; set; }

        /// <summary>
        /// Gets or sets array of tags being assigned to this check.
        /// </summary>
        [JsonProperty(PropertyName = "tags")]
        public IList<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets send an applicant form to applicant to complete to
        /// proceed with check. Defaults to false.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "applicant_provides_data")]
        public bool? ApplicantProvidesData { get; set; }

        /// <summary>
        /// Gets or sets for checks where `applicant_provides_data` is `true`,
        /// applicant form will not be automatically sent if
        /// `suppress_form_emails` is set to `true`. You can manually send the
        /// form at any time after the check has been created, using the link
        /// found in the form_uri attribute of the check object. Write-only.
        /// Defaults to false.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "suppress_form_emails")]
        public bool? SuppressFormEmails { get; set; }

        /// <summary>
        /// Gets or sets defaults to `true`. Write-only. If set to `false`, you
        /// will only receive a response when all reports in your check have
        /// completed.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "asynchronous")]
        public bool? Asynchronous { get; set; }

        /// <summary>
        /// Gets or sets optional. An array of strings describing which
        /// webhooks to trigger for this check. By default, all webhooks
        /// registered in the account will be triggered and this value will be
        /// null in the responses.
        /// </summary>
        [JsonProperty(PropertyName = "webhook_ids")]
        public IList<string> WebhookIds { get; set; }

        /// <summary>
        /// Gets or sets optional. Array of strings describing which document
        /// to process in checks containing a Document report or a Facial
        /// Similarity report, or both. By default, the most recently uploaded
        /// document is used. `document_ids` is only usable with Document and
        /// Facial Similarity reports.
        /// </summary>
        [JsonProperty(PropertyName = "document_ids")]
        public IList<string> DocumentIds { get; set; }

        /// <summary>
        /// Gets for checks where `applicant_provides_data` is `true`, redirect
        /// to this URI when the applicant has submitted their data. Read-only.
        /// </summary>
        [JsonProperty(PropertyName = "redirect_uri")]
        public string RedirectUri { get; private set; }

        /// <summary>
        /// Gets or sets returns a pre-determined consider sub-result in
        /// sandbox for the specific reports in the consider array.
        /// </summary>
        [JsonProperty(PropertyName = "consider")]
        public IList<string> Consider { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (ApplicantId == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ApplicantId");
            }
            if (ReportNames == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ReportNames");
            }
        }
    }
}