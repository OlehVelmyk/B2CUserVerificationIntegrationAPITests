namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class ODataQueryContext
    {
        public string Filter { get; set; }

        public string OrderBy { get; set; }

        public string Top { get; set; }

        public string Skip { get; set; }
    }
}
