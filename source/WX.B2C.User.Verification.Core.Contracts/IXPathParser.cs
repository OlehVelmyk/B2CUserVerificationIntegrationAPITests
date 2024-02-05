using WX.B2C.User.Verification.Domain.XPath;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface IXPathParser
    {
        XPathDetails Parse(string xPath);

        bool IsValid(string xPath);
    }
}