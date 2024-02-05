using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.BlobStorage.Extenstions
{
    public static class StoragePathExtensions
    {
        public static string BlobStoragePath(this BlobFileDto file) =>
            BlobStoragePath(file.UserId, file.FileId, file.FileName);

           public static string BlobStoragePath(Guid userId, Guid fileId, string fileName) =>
            $"{userId}/{fileId.ToString().Replace("-", string.Empty)}/{fileName}";
    }
}