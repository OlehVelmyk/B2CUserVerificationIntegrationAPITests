using System.Collections.Generic;
using System.Linq;
using FsCheck;
using WX.B2C.User.Verification.Domain.XPath;

namespace WX.B2C.User.Verification.Unit.Tests.Arbitraries
{
    public class XPathArbitrary : Arbitrary<XPath>
    {
        public static Arbitrary<XPath> Create()
        {
            return new XPathArbitrary();
        }

        public override Gen<XPath> Generator =>
            from xPath in Gen.Elements(_personal.Concat(_documents).Concat(_verification))
            select xPath;

        private static readonly XPath[] _personal =
        {
            XPathes.FirstName,
            XPathes.LastName,
            XPathes.FullName,
            XPathes.Birthdate,
            XPathes.ResidenceAddress,
            XPathes.PersonalNationality,
            XPathes.Email,
        };

        public static string[] Personal = _personal.Select(path => (string) path).ToArray();

        public static readonly XPath[] _documents =
        {
            XPathes.ProofOfIdentityDocument,
            XPathes.ProofOfAddressDocument,
            XPathes.ProofOfFundsDocument,
            XPathes.SelfieVideo,
            XPathes.SelfiePhoto,
            XPathes.W9Form,
        };

        public static string[] Documents = _documents.Select(path => (string)path).ToArray();

        public static readonly XPath[] _verification =
        {
            XPathes.IpAddress,
            XPathes.TaxResidence,
            XPathes.IdDocumentNumber,
            XPathes.Tin,
        };

        public static string[] Verification = _verification.Select(path => (string)path).ToArray();

        public static readonly XPath[] _subData =
        {
            XPathes.ResidenceAddress.Country,
            XPathes.ResidenceAddress.Line1,
            XPathes.ResidenceAddress.Line2,
            XPathes.ResidenceAddress.City,
            XPathes.ResidenceAddress.State,
            XPathes.ResidenceAddress.ZipCode,
            XPathes.IdDocumentNumber.Number,
            XPathes.IdDocumentNumber.Type,
            XPathes.Tin.Number,
            XPathes.Tin.Type,
        };

        public static string[] SubData = _subData.Select(path => (string)path).ToArray();

        public static readonly string[] Supported = Personal.Concat(Documents).Concat(Verification).ToArray();
    }
}