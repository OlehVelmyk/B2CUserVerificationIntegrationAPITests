namespace WX.B2C.User.Verification.Integration.Tests.Infrastructure;

public class Settings
{
    public EndpointsSection Endpoints { get; set; }
    public UserSection User { get; set; }
}

public class EndpointsSection
{
    public string EnvApi { get; set; }
    public string OnfidoApi { get; set; }
}

public class UserSection
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }

    public string DeviceId { get; set; }
    public string DeviceName { get; set; }
}