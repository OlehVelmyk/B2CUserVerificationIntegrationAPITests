using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure.TestFlow;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.Integration.Tests.Api.Client.Internal.Contracts;
using WX.Preconditions.Extensions;
using WX.Preconditions.Profile;

namespace WX.B2C.User.Verification.Integration.Tests.Steps;

internal class RegisterUserStep : BaseStep
{
    private int? _desiredAge;
    private Address? _address;
    private FullName? _fullname;
    private UserModel _model = null!;
    private UserSection _userSection;

    public RegisterUserStep(UserSection userSection)
    {
        _userSection = userSection;
    }

    public override Task Execute()
    {
        var address = GetAddress();
        var doB = GetDob();
        var profileRequestBuilder = new BaseUserPreconditions("B2CVerification end to end test")
                                    .WithUserFromCountry("int-tests", address.Country)
                                    .WithRequestIpAddressForResidence()
                                    .WithProfile()
                                    .OverrideDateOfBirth(doB)
                                    .OverrideAddress(country: address.Country,
                                                     line1: address.Line1,
                                                     line2: address.Line2,
                                                     zipCode: address.ZipCode,
                                                     city: address.City,
                                                     state: address.State);

        if (_fullname is not null)
            profileRequestBuilder.OverrideName(_fullname.FirstName, _fullname.LastName);
        
        var preconditions = profileRequestBuilder.Done();
        preconditions.BuildPrecondition();
        
        _model = preconditions.UserModels.First();
        return Task.CompletedTask;
    }

    public RegisterUserStep WithAddress(Address address)
    {
        _address = address;
        return this;
    }

    public RegisterUserStep WithAge(int desiredAge)
    {
        _desiredAge = desiredAge;
        return this;
    }

    public RegisterUserStep WithFullName(FullName fullName)
    {
        _fullname = fullName;
        return this;
    }

    public RegisterUserStep WithUserSection(UserSection userSection)
    {
        _userSection = userSection;
        return this;
    }

    public override Task PostCondition()
    {
        Console.WriteLine($"UserId: {_model.UserId}");
        StepContext.FillSignUpSignInData(_userSection);

        StepContext.Instance[General.UserName] = _model.Registration.Email;
        StepContext.Instance[General.Password] = "testuser";
        StepContext.Instance[General.SignUpDeviceName] = "IntegrationTestsAdapter";
        StepContext.Instance[General.SignInDeviceName] = "IntegrationTestsAdapter";
        StepContext.Instance[General.SignInDeviceId] = _model.DeviceId.ToString();
        StepContext.Instance[General.SignUpDeviceId] = _model.DeviceId.ToString();
        StepContext.Instance[General.UserId] = _model.UserId.ToString();
        StepContext.Instance[General.TaxResidence] = GetAddress().Country;
        
        return Task.CompletedTask;
    }

    private Address GetAddress() =>
        _address ?? new Address
        {
            Country = "FR",
            City = "London",
            ZipCode = "75003",
            Line1 = "29 Waterman Way Str. Katharinesd"
        };

    private DateTime GetDob() => 
        DateTime.UtcNow.Subtract(
            TimeSpan.FromDays(365 * (_desiredAge ?? UserAges.LowRiskFactorAge)));
}
