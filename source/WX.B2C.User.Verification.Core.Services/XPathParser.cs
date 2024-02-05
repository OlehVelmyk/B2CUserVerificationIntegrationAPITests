using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Domain.XPath;

namespace WX.B2C.User.Verification.Core.Services
{
    /// <summary>
    /// TODO cover by unit tests
    /// </summary>
    public class XPathParser : IXPathParser
    {
        private readonly IDocumentTypeProvider _documentTypeProvider;
        private readonly Dictionary<string, Func<string, string[], XPathDetails>> _schema;
        private readonly Dictionary<string, Func<string[], bool>> _validationSchema;

        public XPathParser(IDocumentTypeProvider documentTypeProvider)
        {
            _documentTypeProvider = documentTypeProvider;
            _schema = new Dictionary<string, Func<string, string[], XPathDetails>>
            {
                {XPathRoots.PersonalDetails, (xPath, parts) => new PropertyXPathDetails(xPath, PropertySource.Personal, parts[1]) },
                {XPathRoots.VerificationDetails, (xPath, parts) => new PropertyXPathDetails(xPath, PropertySource.Verification, parts[1]) },
                {XPathRoots.Survey, (xPath, parts) => new SurveyXPathDetails(xPath, Guid.Parse(parts[1])) },
                {XPathRoots.Documents, (xPath, parts) =>
                {
                    var category = Enum.Parse<DocumentCategory>(parts[1], true);
                    var type = parts.Length == 3 ? parts[2] : null;
                    return new DocumentsXPathDetails(xPath, category, type);
                } }
            };

            _validationSchema = new Dictionary<string, Func<string[], bool>>
            {
                {XPathRoots.PersonalDetails, (parts) => !string.IsNullOrEmpty(parts[1]) },
                {XPathRoots.VerificationDetails, (parts) => !string.IsNullOrEmpty(parts[1]) },
                {XPathRoots.Survey, (parts) => Guid.TryParse(parts[1], out _) },
                {XPathRoots.Documents, (parts) =>
                {
                    if (!Enum.TryParse<DocumentCategory>(parts[1], false, out var category))
                        return false;
                    if (parts.Length == 3 && !IsDocumentTypeValid(category, parts[2]))
                        return false;

                    return true;
                } }
            };
        }

        public XPathDetails Parse(string xPath)
        {
            var parts = GetParts(xPath);
            var source = parts[0];
            if (!_schema.ContainsKey(source))
                throw new ArgumentOutOfRangeException(nameof(source), source, "Source part of xPath is not recognized");

            return _schema[source](xPath, parts);
        }
        
        /// <summary>
        /// TODO PHASE 2 Revise this logic. Do we need to check XPath at all. This method should be like IsValidCollectionStepXPath.
        /// </summary>
        public bool IsValid(string xPath)
        {
            var parts = GetParts(xPath);
            var source = parts[0];
            if (!_validationSchema.ContainsKey(source))
                return false;

            return _validationSchema[source](parts);
        }

        private bool IsDocumentTypeValid(DocumentCategory category, string documentType) =>
            _documentTypeProvider.IsValid(category, documentType);

        private static string[] GetParts(string xPath)
        {
            var parts = xPath.Split(XPath.Separator);
            if (parts.Length < 2)
                throw new ArgumentException($"xPath must contain at least two documentType: {xPath}", nameof(xPath));

            return parts;
        }
    }
}