using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Optional;
using Optional.Unsafe;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.PassFort.Client.Models;
using WX.B2C.User.Verification.PassFort.Mappers;
using WX.B2C.User.Verification.PassFort.Models;

namespace WX.B2C.User.Verification.PassFort
{
    public interface IPassFortProfilePatcher
    {
        Task<(IndividualData model, bool hasChanges)> ApplyPatch(IndividualData model, PassFortProfilePatch patch);
    }

    internal class PassFortProfilePatcher : IPassFortProfilePatcher
    {
        private readonly IIndividualDataMapper _mapper;
        private readonly Func<IndividualData, PassFortProfilePatch, Task<bool>>[] _schema;

        public PassFortProfilePatcher(IIndividualDataMapper mapper, IPatchDataComparer comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            _schema = new Func<IndividualData, PassFortProfilePatch, Task<bool>>[]
            {
                (model, patch) => Patch(model,
                    patch,
                    p => p.BirthDate,
                    _mapper.MapBirthDate,
                    m => m.PersonalDetails.Dob,
                    string.Equals,
                    (m, newValue) => m.PersonalDetails.Dob = newValue),
                (model, patch) => Patch(model,
                    patch,
                    p => p.FullName,
                    _mapper.MapFullName,
                    m => m.PersonalDetails.Name,
                    comparer.Equals,
                    (m, newValue) => m.PersonalDetails.Name = newValue),
                (model, patch) => Patch(model,
                    patch,
                    p => p.Nationality,
                    _mapper.MapNationalityAsync,
                    m => m.PersonalDetails.Nationality as string,
                    string.Equals,
                    (m, newValue) => m.PersonalDetails.Nationality = newValue),
                (model, patch) => Patch(model,
                    patch,
                    p => p.Email,
                    email => email,
                    m => m.ContactDetails.Email,
                    string.Equals,
                    (m, newValue) => m.ContactDetails.Email = newValue),
                (model, patch) => Patch(model,
                    patch,
                    p => p.Address,
                    PatchAddressHistoryAsync,
                    m => m.AddressHistory,
                    comparer.Equals,
                    (m, newValue) => m.AddressHistory = newValue),
                (model, patch) => Patch(model,
                    patch,
                    p => p.IdDocumentData,
                    PatchDocumentsMetadataAsync,
                    m => m.DocumentsMetadata,
                    (_, _) => false,
                    (m, newValue) => m.DocumentsMetadata = newValue)
            };
        }

        public async Task<(IndividualData model, bool hasChanges)> ApplyPatch(IndividualData model, PassFortProfilePatch patch)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));
            if (patch is null)
                throw new ArgumentNullException(nameof(patch));

            model.ContactDetails ??= new();
            model.PersonalDetails ??= new();

            var hasChanges = false;
            foreach (var applyPatch in _schema)
            {
                hasChanges |= await applyPatch(model, patch);
            }
            return (model, hasChanges);
        }

        private async Task<IList<DatedAddressHistoryItem>> PatchAddressHistoryAsync(AddressDto addressDto)
        {
            var newAddress = new DatedAddressHistoryItem { Address = await _mapper.MapAsync(addressDto) };
            return new List<DatedAddressHistoryItem> { newAddress };
        }

        private async Task<IList<DocumentMetadataIdentityNumber>> PatchDocumentsMetadataAsync((string, IdDocumentNumberDto) documentData)
        {
            var results = new List<DocumentMetadataIdentityNumber>();

            var (issuingCountry, idDocumentNumber) = documentData;
            var documentMetadata = await _mapper.Map(issuingCountry, idDocumentNumber);
            if (documentMetadata is not null)
                results.Add(documentMetadata);

            return results;
        }

        private static Task<bool> Patch<TPatchInput, TModelValue>(IndividualData model,
                                                                  PassFortProfilePatch patch,
                                                                  Func<PassFortProfilePatch, Option<TPatchInput>> patchValue,
                                                                  Func<TPatchInput, TModelValue> mapper,
                                                                  Func<IndividualData, TModelValue> oldValueProvider,
                                                                  Func<TModelValue, TModelValue, bool> comparer,
                                                                  Action<IndividualData, TModelValue> updater) =>
            Patch(model, patch, patchValue, input => Task.FromResult(mapper(input)), oldValueProvider, comparer, updater);

        private static async Task<bool> Patch<TPatchInput, TModelValue>(IndividualData model,
                                                                        PassFortProfilePatch patch,
                                                                        Func<PassFortProfilePatch, Option<TPatchInput>> patchValue,
                                                                        Func<TPatchInput, Task<TModelValue>> mapper,
                                                                        Func<IndividualData, TModelValue> oldValueProvider,
                                                                        Func<TModelValue, TModelValue, bool> comparer,
                                                                        Action<IndividualData, TModelValue> updater)
        {
            var input = patchValue(patch);
            if (!input.HasValue)
                return false;

            var newValue = await mapper(input.ValueOrFailure());
            if (newValue == null)
                return false;

            var oldValue = oldValueProvider(model);
            var areEqual = comparer(newValue, oldValue);
            if (areEqual)
                return false;

            updater(model, newValue);
            return true;
        }
    }
}