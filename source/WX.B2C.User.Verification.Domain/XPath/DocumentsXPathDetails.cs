using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Domain.XPath
{
    public class DocumentsXPathDetails : XPathDetails
    {
        public DocumentsXPathDetails(string xPath, DocumentCategory category, string type) : base(xPath)
        {
            Category = category;
            Type = type;
        }

        public DocumentCategory Category { get; }

        public string Type { get; }

    }
}