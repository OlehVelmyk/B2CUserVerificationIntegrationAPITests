using WX.B2C.User.Profile.Api.Client.Admin.Contracts;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Factories;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Steps.Admin;

internal class AdminUpdateUserFullNameStep : BaseStep
{
    private readonly ProfileAdminApiClientFactory _clientFactory;
    private readonly FullName _fullname;
    
    public AdminUpdateUserFullNameStep(ProfileAdminApiClientFactory clientFactory,
                                       FullName fullname)
    {
        _clientFactory = clientFactory;
        _fullname = fullname;
    }

    public override async Task Execute()
    {
        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var client = await _clientFactory.Create();

        var profileHistory = await client.GetUserProfileHistoryAsync(userId, limit: 1, offset: 0);
        var latestProfile = profileHistory.User_profiles.First();

        var updateModel = Map(latestProfile);
        updateModel.Caused_by = "b2c-verification-int-tests@wirexapp.com";
        updateModel.First_name = _fullname.FirstName;
        updateModel.Last_name = _fullname.LastName;

        await client.UpdateAsync(new UpdateUserProfileRequest { User_profile = updateModel }, $"\"{latestProfile.Version}\"");
    }

    private static AdminUpdateUserProfileModel Map(AdminUserProfileModel model) =>
        new()
        {
            Confirmed_phone_code = model.Confirmed_phone_code,
            Confirmed_phone_number = model.Confirmed_phone_number,
            Created_at = model.Created_at,
            Date_of_birth = model.Date_of_birth,
            Email = model.Email,
            First_name = model.First_name,
            Is_deleted = model.Is_deleted,
            Last_name = model.Last_name,
            Nationality = model.Nationality,
            Nickname = model.Nickname,
            Preferred_locale = model.Preferred_locale,
            Reference_currency = model.Reference_currency,
            Residence_address = model.Residence_address,
            Terms = model.Terms,
            Unconfirmed_phone_code = model.Unconfirmed_phone_code,
            Unconfirmed_phone_number = model.Unconfirmed_phone_number,
            Updated_at = DateTime.UtcNow,
            User_id = model.User_id
        };
}

internal class AdminUpdateUserFullNameStepFactory
{
    private readonly ProfileAdminApiClientFactory _clientFactory;

    public AdminUpdateUserFullNameStepFactory(ProfileAdminApiClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public AdminUpdateUserFullNameStep Create(FullName fullname) =>
        new(_clientFactory, fullname);
}
