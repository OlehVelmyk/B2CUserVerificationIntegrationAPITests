using FsCheck;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Component.Tests.Models.Enums;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators
{
    internal class FilesToUploadGenerators
    {
        public static Gen<FileToUpload[]> Files(int filesNumber, Seed seed) =>
            from documentCategory in Arb.Generate<DocumentCategory>()
            from documentType in DocumentTypeGenerator.Valid()
            from fileName in StringGenerators.LettersOnly(1, 20)
            from fileExtension in Gen.Elements(ValidationRules.FileExtensions)
            let factory = new FileDataFactory(seed)
            from files in Gen.ArrayOf(filesNumber, Generate(documentCategory, documentType, factory))
            select files;

        private static Gen<FileToUpload> Generate(DocumentCategory category,
                                                  string documentType,
                                                  FileDataFactory factory) =>
            from fileName in StringGenerators.LettersOnly(1, 20)
            from fileExtension in Gen.Elements(ValidationRules.FileExtensions)
            select new FileToUpload(category,
                                    documentType,
                                    factory.Create(fileName, fileExtension));
    }
}
