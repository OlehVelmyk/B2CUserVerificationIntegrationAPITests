using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Services.Extensions;
using WX.B2C.User.Verification.Domain.Profile;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Core.Services
{
    internal interface IProfilePatcher
    {
        PropertyChange[] ApplyPatch(PersonalDetailsDto model, PersonalDetailsPatch patch);

        PropertyChange[] ApplyPatch(VerificationDetailsDto model, VerificationDetailsPatch patch);
    }

    internal class ProfilePatcher : IProfilePatcher
    {
        private static readonly Dictionary<string, Func<PersonalDetailsDto, PersonalDetailsPatch, PatchResult>>
            PersonalDetailsPatchSchema = new()
            {
                [XPathes.Email] = (model, patch) =>
                    model.Patch(
                        p => p.Email,
                        () => patch.Email),
                [XPathes.PersonalNationality] = (model, patch) =>
                    model.Patch(
                        p => p.Nationality,
                        () => patch.Nationality),
                [XPathes.Birthdate] = (model, patch) =>
                    model.Patch(
                        p => p.DateOfBirth,
                        () => patch.DateOfBirth.Map(value => value?.Date)),
                [XPathes.ResidenceAddress] = (model, patch) =>
                    model.Patch(
                        p => p.ResidenceAddress,
                        () => patch.ResidenceAddress),
                [XPathes.FirstName] = (model, patch) =>
                    model.Patch(
                        p => p.FirstName,
                        () => patch.FirstName),
                [XPathes.LastName] = (model, patch) =>
                    model.Patch(
                        p => p.LastName,
                        () => patch.LastName),
            };

        private static readonly Dictionary<string, Func<VerificationDetailsDto, VerificationDetailsPatch, PatchResult>>
            VerificationDetailsPatchSchema = new()
            {
                [XPathes.IpAddress] = (model, patch) =>
                    model.Patch(
                        p => p.IpAddress,
                        () => patch.IpAddress),
                [XPathes.IdDocumentNumber] = (model, patch) =>
                    model.Patch(
                        p => p.IdDocumentNumber,
                        () => patch.IdDocumentNumber),
                [XPathes.Turnover] = (model, patch) =>
                    model.Patch(
                        p => p.Turnover,
                        () => patch.Turnover.Map(value => (decimal?)value),
                        turnover => turnover is null or 0,
                        DecimalExtensions.IsEquivalent),
                [XPathes.RiskLevel] = (model, patch) =>
                    model.Patch(
                        p => p.RiskLevel,
                        () => patch.RiskLevel),
                [XPathes.TaxResidence] = (model, patch) =>
                    model.Patch(
                        p => p.TaxResidence,
                        () => patch.TaxResidence,
                        values => values is { Length: 0 },
                        EnumerableExtensions.IsEquivalent),
                [XPathes.Tin] = (model, patch) =>
                    model.Patch(
                        p => p.Tin,
                        () => patch.Tin),
                [XPathes.VerifiedNationality] = (model, patch) =>
                    model.Patch(
                        p => p.Nationality,
                        () => patch.Nationality),
                [XPathes.IsPep] = (model, patch) =>
                    model.Patch(
                        p => p.IsPep,
                        () => patch.IsPep),
                [XPathes.IsSanctioned] = (model, patch) =>
                    model.Patch(
                        p => p.IsSanctioned,
                        () => patch.IsSanctioned),
                [XPathes.IsAdverseMedia] = (model, patch) =>
                    model.Patch(
                        p => p.IsAdverseMedia,
                        () => patch.IsAdverseMedia),
                [XPathes.PoiIssuingCountry] = (model, patch) =>
                    model.Patch(
                        p => p.PoiIssuingCountry,
                        () => patch.PoiIssuingCountry),
                [XPathes.PlaceOfBirth] = (model, patch) =>
                    model.Patch(
                        p => p.PlaceOfBirth,
                        () => patch.PlaceOfBirth),
                [XPathes.ComprehensiveIndex] = (model, patch) =>
                    model.Patch(
                        p => p.ComprehensiveIndex,
                        () => patch.ComprehensiveIndex.Map(value => (int?)value)),
                [XPathes.IsIpMatched] = (model, patch) =>
                    model.Patch(
                        p => p.IsIpMatched,
                        () => patch.IsIpMatched),
                [XPathes.ResolvedCountryCode] = (model, patch) =>
                    model.Patch(
                        p => p.ResolvedCountryCode,
                        () => patch.ResolvedCountryCode)
            };

        public PropertyChange[] ApplyPatch(PersonalDetailsDto model, PersonalDetailsPatch patch)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (patch == null)
                throw new ArgumentNullException(nameof(patch));

            var patchSchema = PersonalDetailsPatchSchema;
            var patchedProperties = ApplyPatch(model, patch, patchSchema).ToList();

            var fullNameChange = GetFullNameChange(model, patchedProperties);
            if (fullNameChange != null) patchedProperties.Add(fullNameChange);

            return patchedProperties.ToArray();
        }

        public PropertyChange[] ApplyPatch(VerificationDetailsDto model, VerificationDetailsPatch patch)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (patch == null)
                throw new ArgumentNullException(nameof(patch));

            var patchSchema = VerificationDetailsPatchSchema;
            var patchedProperties = ApplyPatch(model, patch, patchSchema);

            return patchedProperties.ToArray();
        }

        private static IEnumerable<PropertyChange> ApplyPatch<T, TPatch>(T model, TPatch patch, Dictionary<string, Func<T, TPatch, PatchResult>> patchSchema)
        {
            foreach (var (property, patchAction) in patchSchema)
            {
                var patchResult = patchAction(model, patch);
                if (patchResult.IsPatched) yield return patchResult.GetChange(property);
            }
        }

        private static PropertyChange GetFullNameChange(PersonalDetailsDto model, IList<PropertyChange> patchedProperties)
        {
            var fistNameChange = Find(patchedProperties, XPathes.FirstName);
            var lastNameChange = Find(patchedProperties, XPathes.LastName);
            if (fistNameChange == null && lastNameChange == null) return null;

            var firstNamePreviousValue = fistNameChange is null ? model.FirstName : fistNameChange.PreviousValue;
            var lastNamePreviousValue = lastNameChange is null ? model.LastName : lastNameChange.PreviousValue;

            var newValue = new FullNameDto { FirstName = model.FirstName, LastName = model.LastName };
            var previousValue = firstNamePreviousValue is null && lastNamePreviousValue is null
                ? null
                : new FullNameDto { FirstName = firstNamePreviousValue, LastName = lastNamePreviousValue };
            return PropertyChange.Create(XPathes.FullName, newValue, previousValue);

            static PropertyChange<string> Find(IEnumerable<PropertyChange> props, string name) =>
                (PropertyChange<string>)props.FirstOrDefault(x => x.PropertyName == name);
        }
    }
}
