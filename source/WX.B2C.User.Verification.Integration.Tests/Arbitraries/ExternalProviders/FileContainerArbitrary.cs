using System.IO;
using FsCheck;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.Onfido.Client.Models;
using static WX.B2C.User.Verification.Integration.Tests.Constants.OnfidoFileType;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries.ExternalProviders
{
    internal class DocumentContainerArbitrary : Arbitrary<DocumentContainer>
    {
        public static Arbitrary<DocumentContainer> Create() => new DocumentContainerArbitrary();

        public override Gen<DocumentContainer> Generator =>
            from fileData in FileDataGenerator.FileData()
            where DocumentPredicate(fileData.OnfidoType)
            let data = new FileStream(fileData.Path, FileMode.Open, FileAccess.Read, FileShare.Read)
            let file = new FileToUpload(data, fileData.Name, fileData.ContentType)
            select new DocumentContainer
            {
                File = file,
                MetaData = fileData
            };

        private static bool DocumentPredicate(string onfidoType) =>
            onfidoType is Passport or DrivingLicence;
    }

    internal class LivePhotoContainerArbitrary : Arbitrary<LivePhotoContainer>
    {
        public static Arbitrary<LivePhotoContainer> Create() => new LivePhotoContainerArbitrary();

        public override Gen<LivePhotoContainer> Generator =>
            from fileData in FileDataGenerator.FileData()
            where LivePhotoPredicate(fileData.OnfidoType)
            let data = new FileStream(fileData.Path, FileMode.Open, FileAccess.Read, FileShare.Read)
            let file = new FileToUpload(data, fileData.Name, fileData.ContentType)
            select new LivePhotoContainer
            {
                File = file,
                MetaData = fileData
            };

        private static bool LivePhotoPredicate(string onfidoType) => onfidoType == null;
    }
}
