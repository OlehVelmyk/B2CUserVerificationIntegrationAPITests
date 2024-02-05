using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Onfido.Client.Models;
using WX.B2C.User.Verification.Onfido.Mappers;
using WX.B2C.User.Verification.Onfido.Models;
using WX.B2C.User.Verification.Onfido.Processors.Validators;
using WX.B2C.User.Verification.Provider.Contracts.Exceptions;
using WX.B2C.User.Verification.Provider.Contracts.Models;
using static WX.B2C.User.Verification.Provider.Contracts.Constants;
using CheckDecisions = WX.B2C.User.Verification.Domain.Checks.CheckDecisions;
using TaskExts = WX.B2C.User.Verification.Extensions.TaskExtensions;

namespace WX.B2C.User.Verification.Onfido.Processors
{
    internal interface IIdentityDocumentCheckResultProcessor
    {
        Task<CheckProcessingResult> ProcessAsync(Check check, IList<Report> reports);
    }

    internal class IdentityDocumentCheckResultProcessor : IIdentityDocumentCheckResultProcessor
    {
        private const string NoData = "XX";

        private readonly DocumentReportValidator _reportValidator;
        private readonly ICountryDetailsProvider _countryDetailsProvider;
        private readonly IOnfidoDocumentMapper _documentMapper;
        private readonly ILogger _logger;

        public IdentityDocumentCheckResultProcessor(
            DocumentReportValidator reportValidator,
            ICountryDetailsProvider countryDetailsProvider,
            IOnfidoDocumentMapper documentMapper,
            ILogger logger)
        {
            _reportValidator = reportValidator ?? throw new ArgumentNullException(nameof(reportValidator));
            _countryDetailsProvider = countryDetailsProvider ?? throw new ArgumentNullException(nameof(countryDetailsProvider));
            _documentMapper = documentMapper ?? throw new ArgumentNullException(nameof(documentMapper));
            _logger = logger?.ForContext<IdentityDocumentCheckResultProcessor>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CheckProcessingResult> ProcessAsync(Check check, IList<Report> reports)
        {
            var documentReports = reports.OfType<DocumentReport>().ToArray();
            if (documentReports.Length > 1)
                throw new CheckProcessingException(ErrorCodes.UnsupportedFunctionality, "Multiple reports are not supported.");

            var report = documentReports.SingleOrDefault();
            if (report is null)
                throw new CheckProcessingException(ErrorCodes.ProviderInvalidResponse, "Document report was not found.");

            if (report.Result == CheckResult.Clear)
            {
                var outputData = await CreateOutputDataAsync(report.Properties);
                return CheckProcessingResult.Passed(outputData);
            }
            else
            {
                var (decision, failures) = await ValidateAsync(report);
                var outputData = new IdentityCheckOutputData
                {
                    CheckResult = check.Result,
                    ReportResult = report.Result,
                    Failures = failures
                };
                return CheckProcessingResult.Failed(outputData, decision);
            }
        }

        private async Task<(string, object[])> ValidateAsync(DocumentReport documentReport)
        {
            var validationResult = await _reportValidator.ValidateAsync(documentReport);
            return validationResult.IsValid
                ? (CheckDecisions.Consider, null)
                : (validationResult.GetDecision(), validationResult.GetFailures());
        }

        private async Task<IdentityCheckOutputData> CreateOutputDataAsync(DocumentReportProperties properties)
        {
            if (properties == null)
            {
                return new IdentityCheckOutputData
                {
                    Nationality = NoData,
                    PoiIssuingCountry = NoData,
                    PlaceOfBirth = NoData,
                    IdDocumentNumber = IdDocumentNumberDto.NotPresented
                };
            }

            var documentNumbers = properties.DocumentNumbers?.Where(IsValidDocumentNumber).ToArray();
            if (documentNumbers is not { Length: 1 })
            {
                _logger.Warning(
                    "Suspicious amount {amount} of {documentNumbers} instances was resolved by Onfido: {@data}.",
                    documentNumbers?.Length,
                    nameof(DocumentNumber),
                    documentNumbers);
            }

            var documentNumber = documentNumbers?.FirstOrDefault();
            var idDocumentNumber = documentNumber != null
                ? new IdDocumentNumberDto
                {
                    Number = documentNumber.Value,
                    Type = _documentMapper.MapFromOnfido(properties.DocumentType)
                }
                : IdDocumentNumberDto.NotPresented;

            var (nationality, poiIssuingCountry, placeOfBirth) = await TaskExts.WhenAll(
                ResolveAlpha2CodeAsync(properties.Nationality),
                ResolveAlpha2CodeAsync(properties.IssuingCountry),
                ResolveAlpha2CodeAsync(properties.PlaceOfBirth));

            return new IdentityCheckOutputData
            {
                Nationality = nationality,
                PoiIssuingCountry = poiIssuingCountry,
                PlaceOfBirth = placeOfBirth,
                IdDocumentNumber = idDocumentNumber
            };

            static bool IsValidDocumentNumber(DocumentNumber number) =>
                number.Type == "document_number" && !string.IsNullOrWhiteSpace(number.Value);
        }

        private async Task<string> ResolveAlpha2CodeAsync(string alpha3)
        {
            if (alpha3 == null)
                return NoData;

            return await _countryDetailsProvider.FindAlpha2Async(alpha3) ?? NoData;
        }
    }
}
