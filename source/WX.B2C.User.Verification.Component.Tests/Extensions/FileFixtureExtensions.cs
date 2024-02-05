using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Api.Public.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Fixtures;
using FileToUpload = WX.B2C.User.Verification.Component.Tests.Models.FileToUpload;

namespace WX.B2C.User.Verification.Component.Tests.Extensions
{
    internal static  class FileFixtureExtensions
    {
        public static async Task<string> UploadAsync(this FileFixture fixture,
                                                       Guid userId,
                                                       FileToUpload fileToUpload,
                                                       ExternalFileProviderType? externalProvider = null,
                                                       string externalProfileId = null)
        {
            var fileIds = await fixture.UploadAsync(userId, new[] { fileToUpload }, externalProvider, externalProfileId);
            return fileIds[0];
        }
    }
}
