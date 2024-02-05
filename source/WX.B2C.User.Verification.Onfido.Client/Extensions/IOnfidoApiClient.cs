namespace WX.B2C.User.Verification.Onfido.Client
{
    public partial interface IOnfidoApiClient
    {
        /// <summary>
        /// Gets the IExtractions.
        /// </summary>
        IExtractions Extractions { get; }
    }
}