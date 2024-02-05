namespace WX.B2C.User.Verification.Facade.Controllers.Internal.Requests
{
    public class FacialSimilarityCheckRequest
    {
        public FacialSimilarityCheckVariant Variant { get; set; }
    }

    public enum FacialSimilarityCheckVariant
    {
        Standard = 1,
        Video = 2
    }
}
