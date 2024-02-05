namespace WX.B2C.User.Verification.Domain.XPath
{
    public static class XPathRoots
    {
        public static XPathRoot Survey = new(nameof(Survey));
        public static XPathRoot PersonalDetails = new(nameof(PersonalDetails));
        public static XPathRoot VerificationDetails = new(nameof(VerificationDetails));
        public static XPathRoot Documents = new(nameof(Documents));
    }

    public class XPathRoot
    {
        private readonly string _root;

        internal XPathRoot(string root)
        {
            _root = root;
        }

        public static implicit operator string(XPathRoot xPathRoot) =>
            xPathRoot._root;

        public override string ToString() =>
            _root;
    }
}