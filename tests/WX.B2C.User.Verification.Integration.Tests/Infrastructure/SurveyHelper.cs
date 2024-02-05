using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Infrastructure;

internal static class SurveyHelper
{
    public static Guid GetSurveyId(SurveyType type)
    {
        return type switch
        {
            SurveyType.UsaCdd           => new Guid("de532ca0-c21e-4f7b-ad09-647eaa0c4e00"),
            SurveyType.UsaEdd           => new Guid("eddaca4c-c4a6-40c6-8ff3-d63a5d435783"),
            SurveyType.PepSurvey        => new Guid("ca6b7fb1-413d-449b-9038-32ab5b4914b6"),
            SurveyType.OnboardingSurvey => new Guid("c5e7a138-2e36-43d0-bd76-43a606068f49"),
            SurveyType.OccupationSurvey => new Guid("f9a2a3ac-6e98-43c9-bab2-794e8e6df686"),
            SurveyType.SoFSurvey        => new Guid("0fb7492b-7dc5-4277-a7ff-f3d07376ff66"),
            _                           => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
