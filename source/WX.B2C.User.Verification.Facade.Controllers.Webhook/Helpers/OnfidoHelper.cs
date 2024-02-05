using System;

namespace WX.B2C.User.Verification.Facade.Controllers.Webhook.Helpers
{
    internal class OnfidoHelper
    {
        public static string GetApplicantIdFromLink(string link)
        {
            if (string.IsNullOrEmpty(link))
                return null;

            var index = link.IndexOf("applicants", StringComparison.OrdinalIgnoreCase);
            var data = link.Substring(index);
            var applicantId = data.Split('/')[1];
            return applicantId;
        }
    }
}
