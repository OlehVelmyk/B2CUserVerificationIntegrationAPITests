using System;
using System.Collections.Generic;
using FsCheck;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries.Generators;

namespace WX.B2C.User.Verification.Unit.Tests.Arbitraries
{
    using static StringGenerators;

    internal class TicketInfoArbitrary : Arbitrary<TicketInfo>
    {
        public static Arbitrary<TicketInfo> Create()
        {
            return new TicketInfoArbitrary();
        }

        public override Gen<TicketInfo> Generator =>
            from dateTimeXpath in Arb.Generate<XPath>()
            from stringXpath in Arb.Generate<XPath>().Where(xpath => !xpath.Equals(dateTimeXpath))
            from dateTimeXpathValue in Arb.Generate<DateTime>()
            from stringXpathValue in LettersOnly(1, 10)
            from dateTimeParameterName in LettersOnly(1, 10)
            from stringParameterName in LettersOnly(1, 10)
            from dateTimeParameterValue in Arb.Generate<DateTime>()
            from stringParameterValue in LettersOnly(1, 10)
            select new TicketInfo
            {
                DateTimeXpath = dateTimeXpath,
                StringXpath = stringXpath.ToString(),
                DateTimeXpathValue = dateTimeXpathValue,
                StringXpathValue = stringXpathValue,
                DateTimeParameterName = dateTimeParameterName,
                StringParameterName = stringParameterName,
                DateTimeParameterValue = dateTimeParameterValue,
                StringParameterValue = stringParameterValue
            };
    }

    internal class TicketInfo
    {
        public string DateTimeXpath { get; set; }

        public DateTime DateTimeXpathValue { get; set; }

        public string StringXpath { get; set; }

        public string StringXpathValue { get; set; }

        public string DateTimeParameterName { get; set; }

        public DateTime DateTimeParameterValue { get; set; }

        public string StringParameterName { get; set; }

        public string StringParameterValue { get; set; }

        public Dictionary<string, object> Parameters => new()
        {
            {DateTimeXpath, DateTimeXpathValue},
            {StringXpath, StringXpathValue},
            {DateTimeParameterName, DateTimeParameterValue},
            {StringParameterName, StringParameterValue},
        };
    }
}
