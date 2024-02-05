using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Onfido.Extensions
{
    internal static class DocumentsExtensions
    {
        public static List<string> ExtractFileExternalId(this IEnumerable<DocumentDto> documents)
        {
            if (documents == null)
                throw new ArgumentNullException(nameof(documents));

            return documents.SelectMany(document => document.Files)
                            .Select(file => file.ExternalId)
                            .Distinct()
                            .ToList();
        }
    }
}
