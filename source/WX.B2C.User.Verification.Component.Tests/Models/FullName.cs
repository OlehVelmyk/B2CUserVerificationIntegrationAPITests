namespace WX.B2C.User.Verification.Component.Tests.Models
{
    internal class FullName
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public static FullName Create(string firstName, string lastName) =>
            new() { FirstName = firstName, LastName = lastName };
    }
}