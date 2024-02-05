using FsCheck;
using System.IO;
using WX.B2C.User.Verification.Integration.Tests.Models;
using static WX.B2C.User.Verification.Integration.Tests.Constants;
using static WX.B2C.User.Verification.Integration.Tests.Constants.OnfidoFileType;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries.Generators
{
    internal static class FileDataGenerator
    {
        private static readonly string PathToFolder = Path.Combine(Constants.RootPath, Content.Folder);

        private static readonly FileMetaData[] FileMetaData = new[]
        {
            new FileMetaData
            {
                Name = Content.DrivingLicence,
                Path = Path.Combine(PathToFolder,  Content.DrivingLicence),
                ContentType = "image/png",
                OnfidoType = DrivingLicence
            },
            new FileMetaData
            {
                Name = Content.Passport,
                Path = Path.Combine(PathToFolder, Content.Passport),
                ContentType = "image/jpeg",
                OnfidoType = Passport
            },
            new FileMetaData
            {
                Name = Content.Photo1,
                Path = Path.Combine(PathToFolder,  Content.Photo1),
                ContentType = "image/png",
                OnfidoType = null
            },
            new FileMetaData
            {
                Name = Content.Photo2,
                Path = Path.Combine(PathToFolder, Content.Photo2),
                ContentType = "image/jpeg",
                OnfidoType = null
            }
        };

        public static Gen<FileMetaData> FileData() => Gen.Elements(FileMetaData);
    }
}
