namespace WX.B2C.User.Verification.Component.Tests.Models
{
    internal class Seed
    {
        public int Value { get; set; }

        public static implicit operator int(Seed seed) => seed.Value;
    }
}