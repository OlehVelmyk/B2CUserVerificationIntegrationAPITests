using System;
using FsCheck;
using Optional;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.Utilities;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries
{
    internal class DocumentArbitrary : Arbitrary<DocumentSpecimen>
    {
        public static Arbitrary<DocumentSpecimen> Create() => new DocumentArbitrary();

        public override Gen<DocumentSpecimen> Generator =>
            from id in Arb.Generate<Guid>()
            from userId in Arb.Generate<Guid>()
            from status in Arb.Generate<DocumentStatus>()
            from category in Arb.Generate<DocumentCategory>()
            from type in Arb.Generate<DocumentType>()
            from amountOfFiles in Gen.Choose(0, 5)
            from files in Gen.ArrayOf(amountOfFiles, Arb.Generate<DocumentFileSpecimen>()
                                                        .Override(userId, df => df.File.UserId))
            select new DocumentSpecimen
            {
                Id = id,
                UserId = userId,
                Status = status,
                Category = category,
                Type = type,
                Files = files,
            };
    }

    internal class DocumentFileArbitrary : Arbitrary<DocumentFileSpecimen>
    {
        public static Arbitrary<DocumentFileSpecimen> Create() => new DocumentFileArbitrary();

        public override Gen<DocumentFileSpecimen> Generator =>
            from id in Arb.Generate<Guid>()
            from file in Arb.Generate<FileDto>()
            select new DocumentFileSpecimen
            {
                Id = id,
                File = file
            };
    }

    internal class FileArbitrary : Arbitrary<FileDto>
    {
        public static Arbitrary<FileDto> Create() => new FileArbitrary();

        public override Gen<FileDto> Generator =>
            from id in Arb.Generate<Guid>()
            from userId in Arb.Generate<Guid>()
            from fileName in StringGenerators.NotEmpty(15)
            from status in Arb.Generate<FileStatus>()
            from provider in Arb.Generate<ExternalFileProviderType?>().OrNull()
            from externalId in Arb.Generate<Guid?>().OrNull()
            from checkSum in Gen.Choose(1, int.MaxValue)
            select new FileDto
            {
                Id = id,
                UserId = userId,
                FileName = fileName,
                Status = status,
                Provider = provider,
                Crc32Checksum = ((uint) checkSum).Some(),
                ExternalId = externalId?.ToString()
            };
    }
}
