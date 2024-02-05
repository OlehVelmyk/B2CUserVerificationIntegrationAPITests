namespace WX.B2C.User.Verification.Integration.Tests
{
    public class TheoryAttribute : FsCheck.NUnit.PropertyAttribute
    {
        public TheoryAttribute()
        {
            MaxFail = 1;
            MaxTest = 1;
            QuietOnSuccess = true;
        }
    }
}