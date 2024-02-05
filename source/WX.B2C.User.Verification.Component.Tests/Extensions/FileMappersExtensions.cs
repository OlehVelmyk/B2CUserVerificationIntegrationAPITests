using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Component.Tests.Models.Enums;
using PublicApi = WX.B2C.User.Verification.Api.Public.Client.Models;
using AdminApi = WX.B2C.User.Verification.Api.Admin.Client.Models;
using OnfidoApi = WX.B2C.User.Verification.Onfido.Client.Models;

namespace WX.B2C.User.Verification.Component.Tests.Extensions
{
    internal static class FileMappersExtensions
    {
        public static PublicApi.FileToUpload MapToPublic(this FileData file) =>
            new(file.GetDataCopy(), file.FileName, file.ContentType);

        public static AdminApi.FileToUpload MapToAdmin(this FileData file) =>
            new(file.GetDataCopy(), file.FileName, file.ContentType);

        public static OnfidoApi.FileToUpload MapToOnfido(this FileData file) =>
            new(file.GetDataCopy(), file.FileName, file.ContentType);

        public static FileToUpload Map(this FileData file, DocumentCategory category, string type) =>
            new(category, type, file);
    }
}