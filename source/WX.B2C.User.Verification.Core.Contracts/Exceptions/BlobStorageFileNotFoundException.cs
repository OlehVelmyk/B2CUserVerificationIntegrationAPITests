using System;
using System.Runtime.Serialization;
using WX.B2C.User.Verification.Domain.Exceptions;

namespace WX.B2C.User.Verification.Core.Contracts.Exceptions
{
    [Serializable]
    public class BlobStorageFileNotFoundException : B2CVerificationException
    {
        public BlobStorageFileNotFoundException(Guid fileId)
            : base($"File {fileId} is not found in blob storage for user.")
        {
        }

        public BlobStorageFileNotFoundException(string containerName, string filePath)
            : base($"File by path {filePath} is not found in blob storage.")
        {
            ContainerName = containerName;
            FilePath = filePath;
        }

        protected BlobStorageFileNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));

            FilePath = info.GetString(nameof(FilePath));
            ContainerName = info.GetString(nameof(ContainerName));
        }

        public string FilePath { get; set; }

        public string ContainerName { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(FilePath), FilePath);
            info.AddValue(nameof(ContainerName), ContainerName);

            base.GetObjectData(info, context);
        }
    }
}