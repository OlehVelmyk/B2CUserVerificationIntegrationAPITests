using System;

namespace WX.B2C.User.Verification.Provider.Contracts.Models
{
    public class CheckInputParameter
    {
        private CheckInputParameter(string xPath, bool isRequired)
        {
            if (string.IsNullOrWhiteSpace(xPath))
                throw new ArgumentNullException(nameof(xPath));

            XPath = xPath;
            IsRequired = isRequired;
        }

        public string XPath { get; }

        public bool IsRequired { get; }

        public static CheckInputParameter Required(string xPath) =>
            new(xPath, true);

        public static CheckInputParameter Optional(string xPath) =>
            new(xPath, false);
    }
}