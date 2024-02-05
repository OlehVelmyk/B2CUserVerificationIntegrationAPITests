using System;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class TextNotificationTemplates
    {
        public const string VerificationSuccessful = "Verification successful. Account verification successful";
        public const string VerificationRejected = "Verification rejected. Account verification failed";
        
        public const string DataRequiredTemplate = "Please provide additional information";  

        public static string CreateDocsRequiredTemplate(bool isFirstTime) =>
            $"Please {(isFirstTime ? "submit" : "resubmit")} your docs for continue verification.";
    }

    public class UserResourcesChangedNotificationDto
    {
        public Guid UserId { get; set; }
        
        public Guid CorrelationId { get; set; }
        
        public string[] Created { get; set; }
        
        public string[] Updated { get; set; }
        
        public string[] Deleted { get; set; }
    }

    public class TextNotificationDto
    {
        public Guid UserId { get; set; }

        public Guid CorrelationId { get; set; }

        public string Template { get; set; }

        public string[] TemplateParameters { get; set; }
    }
}