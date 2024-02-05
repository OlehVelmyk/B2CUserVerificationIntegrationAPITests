using System;

namespace WX.B2C.User.Verification.Domain.XPath
{
    public class SurveyXPathDetails : XPathDetails
    {
        public Guid SurveyId { get; }

        public SurveyXPathDetails(string xPath, Guid surveyId) : base(xPath)
        {
            SurveyId = surveyId;
        }
    }
}