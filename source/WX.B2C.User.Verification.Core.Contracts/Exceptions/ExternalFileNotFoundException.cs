using System;
using System.Runtime.Serialization;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.Exceptions;

namespace WX.B2C.User.Verification.Core.Contracts.Exceptions
{
    [Serializable]
    public class ExternalFileNotFoundException : B2CVerificationException
    {
        public ExternalFileNotFoundException(string fileId, string externalProfileId, ExternalFileProviderType provider)
            : base($"File {fileId} is not found for {provider} profile {externalProfileId}.")
        {
            FileId = fileId;
            ProfileId = externalProfileId;
            Provider = provider.ToString();
        }

        protected ExternalFileNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));

            FileId = info.GetString(nameof(FileId));
            ProfileId = info.GetString(nameof(ProfileId));
            Provider = info.GetString(nameof(Provider));
        }

        public string FileId { get; set; }

        public string ProfileId { get; set; }

        public string Provider { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(FileId), FileId);
            info.AddValue(nameof(ProfileId), ProfileId);
            info.AddValue(nameof(Provider), Provider);

            base.GetObjectData(info, context);
        }
    }
}
