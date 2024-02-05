using System;
using WX.B2C.User.Verification.Api.Public.Client.Models;
using DocumentCategory = WX.B2C.User.Verification.Component.Tests.Models.Enums.DocumentCategory;

namespace WX.B2C.User.Verification.Component.Tests.Models
{
    internal class DocumentToSubmit
    {
        public DocumentToSubmit(DocumentCategory documentCategory,
                                string documentType,
                                FileData[] files,
                                ExternalFileProviderType? externalProviderType = null,
                                string externalProfileId = null)
        {
            if (externalProviderType is not null && externalProfileId is null)
                throw new ArgumentNullException(nameof(externalProfileId));

            DocumentCategory = documentCategory;
            DocumentType = documentType ?? throw new ArgumentNullException(nameof(documentType));
            Files = files ?? throw new ArgumentNullException(nameof(files));
            ExternalProviderType = externalProviderType;
            ExternalProfileId = externalProfileId;
        }

        public void SetExternalProvider(ExternalFileProviderType externalProviderType, string externalProfileId)
        {
            ExternalProviderType = externalProviderType;
            ExternalProfileId = externalProfileId;
        }

        public DocumentCategory DocumentCategory { get; }

        public string DocumentType { get; }

        public FileData[] Files { get; }

        public ExternalFileProviderType? ExternalProviderType { get; private set; }

        public string ExternalProfileId { get; private set; }
    }
}