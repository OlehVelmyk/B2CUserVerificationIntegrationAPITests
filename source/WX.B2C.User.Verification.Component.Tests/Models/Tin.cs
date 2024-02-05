namespace WX.B2C.User.Verification.Component.Tests.Models
{
    internal class Tin
    {
        public string Number { get; set; }

        public TinType Type { get; set; }
    }

    internal class InvalidTin : Tin
    { }

    internal enum TinType
    {
        SSN,
        ITIN
    }
}