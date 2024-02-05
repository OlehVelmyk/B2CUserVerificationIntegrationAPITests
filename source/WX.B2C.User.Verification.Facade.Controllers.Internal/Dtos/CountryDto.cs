namespace WX.B2C.User.Verification.Facade.Controllers.Internal.Dtos
{
    public class CountryDto
    {
        public string Name { get; set; }

        public string Alpha2Code { get; set; }

        public string Alpha3Code { get; set; }

        public string PhoneCode { get; set; }

        public string Region { get; set; }

        public bool IsNotSupported { get; set; }

        public bool IsStateRequired { get; set; }

        public StateDto[] States { get; set; }
    }

    public class StateDto
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public bool IsSupported { get; set; }
    }
}
