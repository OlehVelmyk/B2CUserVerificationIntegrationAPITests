using System;

namespace WX.B2C.User.Verification.BlobStorage.Dto
{
    public class CountriesDto
    {
        public CountryDto[] Countries { get; set; } = Array.Empty<CountryDto>();
    }

    public class CountryDto
    {
        public string Alpha2Code { get; set; }

        public string Alpha3Code { get; set; }

        public string Name { get; set; }

        public StateDto[] States { get; set; } = Array.Empty<StateDto>();
    }

    public class StateDto
    {
        public string Code { get; set; }

        public string Name { get; set; }
    }
}