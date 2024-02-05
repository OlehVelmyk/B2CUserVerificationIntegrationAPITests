using System;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Domain.XPath
{
    public class XPath
    {
        public const char Separator = '.';
        private readonly string _xPath;

        private XPath(string xPath)
        {
            _xPath = xPath ?? throw new ArgumentNullException(nameof(xPath));
        }

        public XPath(XPath parent, string current) : this($"{parent}{Separator}{current}") { }
        
        public XPath(XPathRoot parent, string current) : this($"{parent}{Separator}{current}") { }

        public static implicit operator string(XPath xPath) =>
            xPath._xPath;

        public override string ToString() =>
            _xPath;

        protected bool Equals(XPath other)
        {
            return _xPath == other._xPath;
        }

        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj)
         || obj is XPath other && Equals(other)
         || obj is string otherString && otherString == _xPath;

        public override int GetHashCode()
        {
            return (_xPath != null ? _xPath.GetHashCode() : 0);
        }

        public static bool operator ==(XPath left, XPath right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(XPath left, XPath right)
        {
            return !Equals(left, right);
        }
    }

    public class TinXPath : XPath
    {
        public TinXPath() : base(XPathRoots.VerificationDetails, VerificationProperty.Tin)
        {
            Number = new XPath(this, nameof(Number));
            Type = new XPath(this, nameof(Type));
        }

        public XPath Number { get; }

        public XPath Type { get; }
    }

    public class IdDocumentXPath : XPath
    {
        public IdDocumentXPath() : base(XPathRoots.VerificationDetails, VerificationProperty.IdDocumentNumber)
        {
            Number = new XPath(this, nameof(Number));
            Type = new XPath(this, nameof(Type));
        }

        public XPath Number { get; }

        public XPath Type { get; }
    }    
    
    public class AddressXPath : XPath
    {
        public AddressXPath() : base(XPathRoots.PersonalDetails, PersonalProperty.ResidenceAddress)
        {
            Line1 = new XPath(this, nameof(Line1));
            Line2 = new XPath(this, nameof(Line2));
            City = new XPath(this, nameof(City));
            State = new XPath(this, nameof(State));
            Country = new XPath(this, nameof(Country));
            ZipCode = new XPath(this, nameof(ZipCode));
        }

        public XPath Line1 { get; }

        public XPath Line2 { get; }

        public XPath City { get; }

        public XPath State { get; }

        public XPath Country { get; }

        public XPath ZipCode { get; }
    }

    public class DocumentXPath : XPath
    {
        public DocumentXPath(DocumentCategory documentCategory): this(documentCategory, null)
        {
            
        }

        public DocumentXPath(DocumentCategory documentCategory, string documentType) : base(XPathRoots.Documents, BuildDocumentPart(documentCategory, documentType))
        {

        }

        private static string BuildDocumentPart(DocumentCategory documentCategory, string type) =>
            type switch
            {
                nameof(TaxationDocumentType.W9Form) => $"{documentCategory}{Separator}{type}",
                nameof(SelfieDocumentType.Video)    => $"{documentCategory}{Separator}{type}",
                nameof(SelfieDocumentType.Photo)    => $"{documentCategory}{Separator}{type}",
                _                                   => $"{documentCategory}"
            };
    }

    public class SurveyXPath : XPath
    {
        public SurveyXPath(Guid templateId) : base(XPathRoots.Survey, templateId.ToString().ToUpperInvariant())
        {

        }
    }
}