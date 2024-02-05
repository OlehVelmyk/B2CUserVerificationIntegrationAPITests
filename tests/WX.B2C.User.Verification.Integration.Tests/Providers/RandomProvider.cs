namespace WX.B2C.User.Verification.Integration.Tests.Providers;

internal static class RandomProvider
{
    public static string GenerateString() =>
        new(Guid.NewGuid().ToString().Where(char.IsLetter).ToArray());
    
    public static string GenerateIdDocNumber()
    {
        var rnd = new Random();
        var length = rnd.Next(8, 15);
        var charArray = Enumerable.Range(0, length).Select(_ => (char)rnd.Next(48, 57)).ToArray();
        return new string(charArray);
    }
}
