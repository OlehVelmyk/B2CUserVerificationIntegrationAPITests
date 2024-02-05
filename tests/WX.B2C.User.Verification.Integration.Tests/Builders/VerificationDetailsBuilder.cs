using WX.B2C.User.Verification.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Builders;

internal class AdminUpdateVerificationDetailsRequestBuilder
{
    private bool? _isAdverseMedia;
    private bool? _isSanctioned;
    private bool? _isPep;
    private IdDocumentNumberDto? _idDocumentNumber;
    private string _reason = "End to End testing";

    public AdminUpdateVerificationDetailsRequestBuilder With(string reason)
    {
        _reason = reason;
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

    public AdminUpdateVerificationDetailsRequestBuilder WithIdDocNumber(string number, string type)
    {
        _idDocumentNumber = new IdDocumentNumberDto()
        {
            Number = number,
            Type = type
        };
        return this;
    }

    public UpdateVerificationDetailsRequest Build() =>
        new()
        {
            Reason = _reason,
            IsAdverseMedia = _isAdverseMedia,
            IsSanctioned = _isSanctioned,
            IsPep = _isPep,
            IdDocumentNumber = _idDocumentNumber
        };
}
