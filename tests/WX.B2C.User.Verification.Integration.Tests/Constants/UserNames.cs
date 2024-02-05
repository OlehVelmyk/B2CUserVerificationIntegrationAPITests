using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Constants;

internal static class UserNames
{
    public static FullName UsaSuccessfulFlowUserName =>
        new()
        {
            FirstName = "Flostri",
            LastName = "Cocomeow"
        };

    public static FullName UsaNotResidentUserName =>
        new()
        {
            FirstName = "Katrin",
            LastName = "Donnovan"
        };

    public static FullName UsaPepUserName =>
        new()
        {
            FirstName = "Sebastian",
            LastName = "Pereira"
        };
    
    public static FullName SanctionedUserName =>
        new()
        {
            FirstName = "Tom Sanction",
            LastName = "Smith"
        };

    public static FullName PepUserName =>
        new()
        {
            FirstName = "Tom PEP",
            LastName = "Smith"
        };

    public static FullName OnfidoChecksConsiderUserName =>
        new()
        {
            FirstName = "AnyFirstName",
            LastName = "Consider"
        };

    public static FullName IdentityDocumentCheckFailedWithDecisionResubmitUserName =>
        new()
        {
            FirstName = "Image Integrity - Image Quality",
            LastName = "AnyLastName"
        };

    public static FullName FacialSimilarityCheckFailedWithDecisionResubmitUserName =>
        new()
        {
            FirstName = "Face Comparison - Face Match",
            LastName = "LastName"
        };

    public static FullName IdentityDocumentCheckFailedWithDecisionFraudUserName =>
        new()
        {
            FirstName = "Visual Authenticity - Security Features",
            LastName = "LastName"
        };
}
