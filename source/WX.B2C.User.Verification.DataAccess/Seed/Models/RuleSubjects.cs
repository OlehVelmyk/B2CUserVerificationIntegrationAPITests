using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.DataAccess.Seed.Models
{
    /// <summary>
    /// Possible rule subjects excluding <see cref="DocumentType"/>
    /// </summary>
    internal static class RuleSubjects
    {
        public const string Tin = nameof(Tin);
        public const string TaxResidence = nameof(TaxResidence);
    }
}