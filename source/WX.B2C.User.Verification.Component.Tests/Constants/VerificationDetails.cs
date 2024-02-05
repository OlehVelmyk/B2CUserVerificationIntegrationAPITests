using System;
using WX.B2C.User.Verification.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Component.Tests.Constants
{
    internal static class VerificationDetails
    {
        public static readonly string IpAddress = $"{nameof(VerificationDetails)}.{nameof(IpAddress)}";
        public static readonly string TaxResidence = $"{nameof(VerificationDetails)}.{nameof(TaxResidence)}";
        public static readonly string Tin = $"{nameof(VerificationDetails)}.{nameof(Tin)}";
        public static readonly string Turnover = $"{nameof(VerificationDetails)}.{nameof(Turnover)}";
        public static readonly string IdDocumentNumber = $"{nameof(VerificationDetails)}.{nameof(IdDocumentNumber)}";
        public static readonly string IsPep = $"{nameof(VerificationDetails)}.{nameof(IsPep)}";
        public static readonly string IsAdverseMedia = $"{nameof(VerificationDetails)}.{nameof(IsAdverseMedia)}";
        public static readonly string IsSanctioned = $"{nameof(VerificationDetails)}.{nameof(IsSanctioned)}";
        public static readonly string RiskLevel = $"{nameof(VerificationDetails)}.{nameof(RiskLevel)}";
        public static readonly string Nationality = $"{nameof(VerificationDetails)}.{nameof(Nationality)}";
        public static readonly string IsIpMatched = $"{nameof(VerificationDetails)}.{nameof(IsIpMatched)}";


        public static string GetXPath(this VerificationDetailsProperty verificationDetailsProperty) =>
            verificationDetailsProperty switch
            {
                VerificationDetailsProperty.IpAddress => IpAddress,
                VerificationDetailsProperty.TaxResidence => TaxResidence,
                VerificationDetailsProperty.RiskLevel => RiskLevel,
                VerificationDetailsProperty.IdDocumentNumber => IdDocumentNumber,
                VerificationDetailsProperty.Tin => Tin,
                VerificationDetailsProperty.Nationality => Nationality,
                VerificationDetailsProperty.IsPep => IsPep,
                VerificationDetailsProperty.IsSanctioned => IsSanctioned,
                VerificationDetailsProperty.IsAdverseMedia => IsAdverseMedia,
                VerificationDetailsProperty.Turnover => Turnover,
                VerificationDetailsProperty.IsIpMatched => IsIpMatched,
                _ => throw new ArgumentOutOfRangeException(nameof(verificationDetailsProperty), verificationDetailsProperty, null)
            };
    }
}