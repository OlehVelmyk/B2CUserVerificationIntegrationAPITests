namespace WX.B2C.User.Verification.Component.Tests
{
    internal class TheoryAttribute : FsCheck.NUnit.PropertyAttribute
    {
        public TheoryAttribute()
        {
            MaxFail = 1;
            MaxTest = 1;
            QuietOnSuccess = true;
        }
    }
}