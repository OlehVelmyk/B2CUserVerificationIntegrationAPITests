using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Constants;

internal static class UserAddresses
{
    public static Address Gb => new()
    {
        Country = "GB",
        Line1 = "29 Waterman Way Str. Katharinesd",
        ZipCode = "SW1Y 6QY",
        City = "London"
    };

    public static Address Tr => new()
    {
        Line1 = "29 Waterman Way St. Katharines",
        Country = "TR",
        ZipCode = "00004",
        City = "Rabat"
    };

    public static Address Au => new()
    {
        Country = "AU",
        State = "NT"
    };

    public static Address Ph => new()
    {
        Country = "PH",
        City = "Manila",
        ZipCode = "00004",
        Line1 = "29 Waterman Way Str. Katharinesd"
    };

    public static Address Nl => new()
    {
        Country = "NL",
        ZipCode = "1111",
        City = "Diemen",
        Line1 = "Kriekenoord 3"
    };

    public static Address Md => new()
    {
        Country = "MD",
        Line1 = "CUCU VASILE STR. EMINESCU",
        ZipCode = "MD-6812",
        City = "Chisinau",
    };
    
    public static Address Us => new()
    {
        Country = "US",
        Line1 = "96 Vanira Ave SE",
        ZipCode = "30315",
        City = "Atlanta",
        State = "GA"
    };

    public static Address UsNotResident => new()
    {
        Country = "US",
        Line1 = "301 Marshall St",
        ZipCode = "59301",
        City = "Miles City",
        State = "MT"
    };

    public static Address UsPepUserAddress => new()
    {
        Country = "US",
        Line1 = "9234 Hannover Dr",
        ZipCode = "99581",
        City = "Emmonak",
        State = "AK"
    };

    public static Address GbWithIdDocNumber(string number) => new()
    {
        Country = "GB",
        Line1 = "29 Waterman Way Str. Katharinesd",
        Line2 = $"IdDoc:{number}",
        ZipCode = "SW1Y 6QY",
        City = "London"
    };
}
