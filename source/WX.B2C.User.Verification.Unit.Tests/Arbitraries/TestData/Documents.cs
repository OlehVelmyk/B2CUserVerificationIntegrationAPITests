using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Unit.Tests.Arbitraries.TestData
{
    public class Documents
    {
        public Documents()
        {
            ProofOfIdentity = new DocumentDto
            {
                Category = DocumentCategory.ProofOfIdentity,
                Id = Guid.NewGuid(),
                Status = DocumentStatus.Submitted,
                CreatedAt = DateTime.UtcNow,
            };
            ProofOfAddress = new DocumentDto
            {
                Category = DocumentCategory.ProofOfAddress,
                Id = Guid.NewGuid(),
                Status = DocumentStatus.Submitted,
                CreatedAt = DateTime.UtcNow,
            };
            ProofOfFunds = new DocumentDto
            {
                Category = DocumentCategory.ProofOfFunds,
                Id = Guid.NewGuid(),
                Status = DocumentStatus.Submitted,
                CreatedAt = DateTime.UtcNow,
            };
            SelfiePhoto = new DocumentDto
            {
                Category = DocumentCategory.Selfie,
                Type = SelfieDocumentType.Photo,
                Id = Guid.NewGuid(),
                Status = DocumentStatus.Submitted,
                CreatedAt = DateTime.UtcNow,
            };             
            SelfieVideo = new DocumentDto
            {
                Category = DocumentCategory.Selfie,
                Type = SelfieDocumentType.Video,
                Id = Guid.NewGuid(),
                Status = DocumentStatus.Submitted,
                CreatedAt = DateTime.UtcNow,
            }; 
            W9Form = new DocumentDto
            {
                Category = DocumentCategory.Taxation,
                Type = TaxationDocumentType.W9Form,
                Id = Guid.NewGuid(),
                Status = DocumentStatus.Submitted,
                CreatedAt = DateTime.UtcNow,
            };
        }

        public DocumentDto W9Form { get;}

        public DocumentDto SelfiePhoto { get; }

        public DocumentDto SelfieVideo { get; }

        public DocumentDto ProofOfFunds { get; }

        public DocumentDto ProofOfAddress { get; }

        public DocumentDto ProofOfIdentity { get; }

        public DocumentDto[] All => new[] { ProofOfIdentity, ProofOfAddress, ProofOfFunds, SelfiePhoto, SelfieVideo, W9Form };
    }
}
