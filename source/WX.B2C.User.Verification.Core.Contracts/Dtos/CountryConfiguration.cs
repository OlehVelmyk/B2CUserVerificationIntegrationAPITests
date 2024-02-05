namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class CountryConfiguration
    {
        public string Alpha2 { get; set; }

        public string Alpha3 { get; set; }

        public string Name { get; set; }

        public string Region { get; set; }
    }

    public class StateConfiguration
    {
        public string Alpha2 { get; set; }

        public string Name { get; set; }

        public string CountryAlpha2 { get; set; }

        public bool IsSupported { get; set; }
    }
}