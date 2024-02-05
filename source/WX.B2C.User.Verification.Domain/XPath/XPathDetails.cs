namespace WX.B2C.User.Verification.Domain.XPath
{
    /// <summary>
    /// TODO PHASE2 remove as later XPath class can be used.
    /// </summary>
    public abstract class XPathDetails
    {
        public string XPath { get; }

        protected XPathDetails(string xPath)
        {
            XPath = xPath;
        }
    }
}