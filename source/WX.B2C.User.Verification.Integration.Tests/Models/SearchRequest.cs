using BridgerReference;

namespace WX.B2C.User.Verification.Integration.Tests.Models
{
    public class SearchRequest
    {
        public string[] SearchNames { get; set; }

        public SearchInput SearchInput { get; set; }
    }

    public class PepSearchRequest : SearchRequest
    {  }
}
