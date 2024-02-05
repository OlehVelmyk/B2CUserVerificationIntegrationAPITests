using System.Collections.Generic;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Extensions;
using TinType = WX.B2C.User.Verification.Api.Admin.Client.Models.TinType;

namespace WX.B2C.User.Verification.Component.Tests.Builders
{
    internal class AdminUpdateVerificationDetailsRequestBuilder
    {
        private TinDto _tin = null;
        private IList<string> _taxResidence = null;
        private IdDocumentNumberDto _idDocumentNumber = null;
        private bool? _isAdverseMedia = null;
        private bool? _isSanctioned = null;
        private bool? _isPep = null;
        private string _reason = "Component testing";

        public AdminUpdateVerificationDetailsRequestBuilder With(string reason)
        {
            _reason = reason;
            return this;
        }

        public AdminUpdateVerificationDetailsRequestBuilder With(TaxResidence taxResidence)
        {
            _taxResidence = taxResidence.Countries;
            return this;
        }

        public AdminUpdateVerificationDetailsRequestBuilder With(string[] taxResidence)
        {
            _taxResidence = taxResidence;
            return this;
        }

        public AdminUpdateVerificationDetailsRequestBuilder With(Tin tin)
        {
            _tin = new TinDto
            {
                Number = tin.Number,
                Type = tin.Type.To<TinType>()
            };
            return this;
        }

        public AdminUpdateVerificationDetailsRequestBuilder With(IdDocumentNumber idDocumentNumber)
        {
            _idDocumentNumber = new IdDocumentNumberDto
            {
                Type = idDocumentNumber.Type,
                Number = idDocumentNumber.Number
            };
            return this;
        }

        public AdminUpdateVerificationDetailsRequestBuilder WithIsAdverseMedia(bool isAdverseMedia)
        {
            _isAdverseMedia = isAdverseMedia;
            return this;
        }

        public AdminUpdateVerificationDetailsRequestBuilder WithIsSanctioned(bool isSanctioned)
        {
            _isSanctioned = isSanctioned;
            return this;
        }

        public AdminUpdateVerificationDetailsRequestBuilder WithIsPep(bool isPep)
        {
            _isPep = isPep;
            return this;
        }

        public UpdateVerificationDetailsRequest Build() =>
            new()
            {
                Reason = _reason,
                TaxResidence = _taxResidence,
                Tin = _tin,
                IdDocumentNumber = _idDocumentNumber,
                IsAdverseMedia = _isAdverseMedia,
                IsSanctioned = _isSanctioned,
                IsPep = _isPep
            };
    }
}