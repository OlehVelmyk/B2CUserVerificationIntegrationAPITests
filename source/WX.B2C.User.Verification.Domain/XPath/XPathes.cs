using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Domain.XPath
{
    /// <summary>
    /// Will be renamed in next PR to XPathes.
    /// </summary>
    public static class XPathes
    {
        public static readonly XPath IpAddress = new(XPathRoots.VerificationDetails, VerificationProperty.IpAddress);
        public static readonly XPath TaxResidence = new(XPathRoots.VerificationDetails, VerificationProperty.TaxResidence);
        public static readonly XPath RiskLevel = new(XPathRoots.VerificationDetails, VerificationProperty.RiskLevel);
        public static readonly IdDocumentXPath IdDocumentNumber = new();
        public static readonly TinXPath Tin = new();
        public static readonly XPath Turnover = new(XPathRoots.VerificationDetails, VerificationProperty.Turnover);
        public static readonly XPath VerifiedNationality = new(XPathRoots.VerificationDetails, VerificationProperty.Nationality);
        public static readonly XPath IsPep = new(XPathRoots.VerificationDetails, VerificationProperty.IsPep);
        public static readonly XPath IsSanctioned = new(XPathRoots.VerificationDetails, VerificationProperty.IsSanctioned);
        public static readonly XPath IsAdverseMedia = new(XPathRoots.VerificationDetails, VerificationProperty.IsAdverseMedia);
        public static readonly XPath PoiIssuingCountry = new(XPathRoots.VerificationDetails, VerificationProperty.PoiIssuingCountry);
        public static readonly XPath PlaceOfBirth = new(XPathRoots.VerificationDetails, VerificationProperty.PlaceOfBirth);
        public static readonly XPath ComprehensiveIndex = new(XPathRoots.VerificationDetails, VerificationProperty.ComprehensiveIndex);
        public static readonly XPath IsIpMatched = new(XPathRoots.VerificationDetails, VerificationProperty.IsIpMatched);
        public static readonly XPath ResolvedCountryCode = new(XPathRoots.VerificationDetails, VerificationProperty.ResolvedCountryCode);

        public static readonly XPath FirstName = new(XPathRoots.PersonalDetails, PersonalProperty.FirstName);
        public static readonly XPath LastName = new(XPathRoots.PersonalDetails, PersonalProperty.LastName);
        public static readonly XPath FullName = new(XPathRoots.PersonalDetails, PersonalProperty.FullName);
        public static readonly XPath Birthdate = new(XPathRoots.PersonalDetails, PersonalProperty.Birthdate);
        public static readonly AddressXPath ResidenceAddress = new();
        public static readonly XPath PersonalNationality = new(XPathRoots.PersonalDetails, PersonalProperty.Nationality);
        public static readonly XPath Email = new(XPathRoots.PersonalDetails, PersonalProperty.Email);
        public static readonly XPath ProfileCreationDate = new(XPathRoots.PersonalDetails, PersonalProperty.CreatedAt);

        public static readonly XPath SelfieVideo = new DocumentXPath(DocumentCategory.Selfie, SelfieDocumentType.Video);
        public static readonly XPath SelfiePhoto = new DocumentXPath(DocumentCategory.Selfie, SelfieDocumentType.Photo);
        public static readonly XPath W9Form = new DocumentXPath(DocumentCategory.Taxation, TaxationDocumentType.W9Form);
        public static readonly XPath ProofOfIdentityDocument = new DocumentXPath(DocumentCategory.ProofOfIdentity);
        public static readonly XPath ProofOfAddressDocument = new DocumentXPath(DocumentCategory.ProofOfAddress);
        public static readonly XPath ProofOfFundsDocument = new DocumentXPath(DocumentCategory.ProofOfFunds);
    }
}