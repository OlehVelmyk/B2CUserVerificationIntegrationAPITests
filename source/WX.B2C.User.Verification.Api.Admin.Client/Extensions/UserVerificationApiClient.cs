using Microsoft.Rest;

namespace WX.B2C.User.Verification.Api.Admin.Client
{

    public partial class UserVerificationApiClient : ServiceClient<UserVerificationApiClient>, IUserVerificationApiClient
    {
        /// <summary>
        /// Gets the IFiles.
        /// </summary>
        public virtual IFiles Files { get; private set; }

        partial void CustomInitialize()
        {
            Files = new Files(this);
        }
    }
}