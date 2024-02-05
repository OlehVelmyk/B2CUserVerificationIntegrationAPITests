using System.Collections.Generic;
using WX.B2C.User.Verification.Api.Public.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Extensions;
using TinType = WX.B2C.User.Verification.Api.Public.Client.Models.TinType;

namespace WX.B2C.User.Verification.Component.Tests.Builders
{
    internal class UserUpdateVerificationDetailsRequestBuilder
    {
        private TinDto _tin = null;
        private IList<string> _taxResidence = null;

        public UserUpdateVerificationDetailsRequestBuilder With(TaxResidence taxResidence)
        {
            _taxResidence = taxResidence.Countries;
            return this;
        }

        public UserUpdateVerificationDetailsRequestBuilder With(string[] taxResidence)
        {
            _taxResidence = taxResidence;
            return this;
        }

        public UserUpdateVerificationDetailsRequestBuilder With(Tin tin)
        {
            _tin = new TinDto()
            {
                Number = tin.Number,
                Type = tin.Type.To<TinType>()
            };
            return this;
        }

        public UpdateVerificationDetailsRequest Build() =>
            new UpdateVerificationDetailsRequest()
            {
                TaxResidence = _taxResidence,
                Tin = _tin
            };
    }
}