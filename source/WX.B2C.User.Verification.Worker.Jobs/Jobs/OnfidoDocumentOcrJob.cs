using System;
using System.Threading;
using System.Threading.Tasks;
using Optional;
using Quartz;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Onfido.Client.Models;
using WX.B2C.User.Verification.Onfido.Constants;
using WX.B2C.User.Verification.Worker.Jobs.Clients;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Providers;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs
{
    internal sealed class OnfidoDocumentOcrJob : BatchJob<OnfidoFileData, OnfidoDocumentOcrJobSetting>
    {
        private const string NoData = "XX";

        private readonly IThrottledOnfidoApiClientFactory _onfidoApiClientFactory;
        private readonly ICountryDetailsProvider _countryDetailsProvider;
        private readonly IProfileService _profileService;

        public OnfidoDocumentOcrJob(
            IThrottledOnfidoApiClientFactory onfidoApiClientFactory,
            IOnfidoFileDataProvider jobDataProvider,
            ICountryDetailsProvider countryDetailsProvider,
            IProfileService profileService,
            ILogger logger)
            : base(jobDataProvider, logger)
        {
            _onfidoApiClientFactory = onfidoApiClientFactory ?? throw new ArgumentNullException(nameof(onfidoApiClientFactory));
            _countryDetailsProvider = countryDetailsProvider ?? throw new ArgumentNullException(nameof(countryDetailsProvider));
            _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
        }

        public static string Name => "onfido-document-ocr";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<OnfidoDocumentOcrJob>()
                 .WithIdentity(Name, "Backfill")
                 .WithDescription("Updates the document OCR from onfido.");

        protected override async Task Execute(Batch<OnfidoFileData> batch, OnfidoDocumentOcrJobSetting settings, CancellationToken cancellationToken)
        {
            var apiClient = _onfidoApiClientFactory.Create(settings.OnfidoRequestPerMinute);

            foreach (var file in batch.Items)
            {
                if (!file.Provider.HasValue || file.ExternalId is null)
                    continue;

                var verificationDetailsPatch = await GetVerificationDetailsPatch(apiClient, file);
                if (verificationDetailsPatch == null)
                {
                    continue;
                }

                await ValidationDetailsUpdate(file, verificationDetailsPatch);
            }
        }

        private async Task<VerificationDetailsPatch> GetVerificationDetailsPatch(IOnfidoApiClientWithThrottling onfidoApiClientWithThrottling, OnfidoFileData file)
        {
            var extractionRequest = new ExtractionRequest(file.ExternalId);

            try
            {
                var extraction = await onfidoApiClientWithThrottling.GetExtractions(extractionRequest);

                return await GetVerificationDetailsPatch(extraction);
            }
            catch (Exception ex)
            {
                base.Logger.Error(ex, "Onfido Extractions request ExternalId {item.ExternalId} error", file.ExternalId);
                IncrementErrorCount();
            }

            return null;
        }

        private async Task<VerificationDetailsPatch> GetVerificationDetailsPatch(Extraction extraction)
        {
            var idDocumentNumberDto = string.IsNullOrWhiteSpace(extraction.ExtractedData.DocumentNumber)
                ? IdDocumentNumberDto.NotPresented
                : new IdDocumentNumberDto
                {
                    Number = extraction.ExtractedData.DocumentNumber,
                    Type = Map(extraction.ExtractedData.DocumentType)
                };

            return new VerificationDetailsPatch
            {
                // first from array of document numbers
                IdDocumentNumber = idDocumentNumberDto.SomeNotNull(),

                // map to country alpha2 code or XX(find by iso-3)
                Nationality = (await ResolveAlpha2CodeAsync(extraction.ExtractedData.Nationality)).SomeNotNull(),

                // map to country alpha2 or XX(find by country name)
                PlaceOfBirth = (await ResolveAlpha2CodeAsync(extraction.ExtractedData.PlaceOfBirth) ?? extraction.ExtractedData.PlaceOfBirth).SomeNotNull(),

                // map to country alpha2 code or XX(find by iso-3)
                PoiIssuingCountry = (await ResolveAlpha2CodeAsync(extraction.DocumentClassification.IssuingCountry)).SomeNotNull()
            };
        }

        private async Task ValidationDetailsUpdate(OnfidoFileData file, VerificationDetailsPatch verificationDetailsPatch)
        {
            try
            {
                var initiationDto = InitiationDto.CreateSystem($"Update verification details from job: {Name} for userId: {file.UserId}");
                await _profileService.UpdateAsync(file.UserId, verificationDetailsPatch, initiationDto);
            }
            catch (Exception ex)
            {
                base.Logger.Error(ex, $"Job {Name}, verificationDetails update for userId: {file.UserId}");
                IncrementErrorCount();
            }
        }

        private async Task<string> ResolveAlpha2CodeAsync(string alpha3)
        {
            if (alpha3 == null)
                return NoData;

            return await _countryDetailsProvider.FindAlpha2Async(alpha3) ?? NoData;
        }

        private static DocumentType Map(string documentType) =>
            documentType switch
            {
                OnfidoDocumentType.DrivingLicence => IdentityDocumentType.DriverLicense,
                OnfidoDocumentType.Passport => IdentityDocumentType.Passport,
                OnfidoDocumentType.NationalIdentityCard => IdentityDocumentType.IdentityCard,
                OnfidoDocumentType.BankBuildingSocietyStatement => AddressDocumentType.BankStatement,
                OnfidoDocumentType.UtilityBill => AddressDocumentType.UtilityBill,
                OnfidoDocumentType.CouncilTax => AddressDocumentType.CouncilTax,
                OnfidoDocumentType.BenefitLetters => AddressDocumentType.TaxReturn,
                _ => IdentityDocumentType.Other
            };
    }
}
