namespace WX.B2C.User.Verification.Domain.XPath
{
    public class PropertyXPathDetails : XPathDetails
    {
        public PropertySource Source { get; }

        public string Property { get; }

        public PropertyXPathDetails(string xPath, PropertySource source, string property) : base(xPath)
        {
            Source = source;
            Property = property;
        }
    }
}