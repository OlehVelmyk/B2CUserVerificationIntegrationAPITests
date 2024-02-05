using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Core.Services.Validation
{
    public class ValidationRuleFilterContext
    {
        public Guid UserId { get; set; }

        public string Country { get; set; }

        public ActionType ActionType { get; set; }
    }

    public interface IValidationRuleFilter
    {
        bool CanApply(ValidationRuleFilterContext context);

        Task<ValidationRuleDto> ApplyAsync(ValidationRuleDto validationRule, ValidationRuleFilterContext context);
    }

    public class FundsDocumentValidationRuleFilter : IValidationRuleFilter
    {
        private const string FundsOriginQuestionTag = "funds_origin";
        private readonly Guid _sofSurveyId = Guid.Parse("0FB7492B-7DC5-4277-A7FF-F3D07376FF66");

        private readonly IUserSurveyProvider _userSurveyProvider;

        public FundsDocumentValidationRuleFilter(IUserSurveyProvider userSurveyProvider)
        {
            _userSurveyProvider = userSurveyProvider ?? throw new ArgumentNullException(nameof(userSurveyProvider));
        }

        public bool CanApply(ValidationRuleFilterContext context) => context.Country == "GB" && context.ActionType == ActionType.ProofOfFunds;

        public async Task<ValidationRuleDto> ApplyAsync(ValidationRuleDto validationRule, ValidationRuleFilterContext context)
        {
            if (validationRule == null)
                throw new ArgumentNullException(nameof(validationRule));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (validationRule is not DocumentValidationRuleDto documentValidationRule)
                return validationRule;

            var allowedTypes = await GetAllowedDocumentTypesAsync(context.UserId, documentValidationRule);
            documentValidationRule.AllowedTypes = allowedTypes;
            return documentValidationRule;
        }

        private async Task<Dictionary<string, DocumentTypeValidationRuleDto>> GetAllowedDocumentTypesAsync(Guid userId, DocumentValidationRuleDto documentValidationRule)
        {
            var taggedAnswers = await _userSurveyProvider.GetAnswersAsync(userId, _sofSurveyId, new[] { FundsOriginQuestionTag });
            if (taggedAnswers.Count == 0)
                return new Dictionary<string, DocumentTypeValidationRuleDto>();

            var sofSurveyAnswers = taggedAnswers.First().Values.ToArray();
            var allowedSofDocumentTypes = MapSofSurveyAnswersToDocumentTypes(sofSurveyAnswers);

            return documentValidationRule
                   .AllowedTypes
                   .Where(rule => allowedSofDocumentTypes.Contains(rule.Key))
                   .ToDictionary(x => x.Key, x => x.Value);
        }

        private static IEnumerable<string> MapSofSurveyAnswersToDocumentTypes(IReadOnlyCollection<string> answers)
        {
            if (answers.Count == 0)
                return Array.Empty<string>();

            var sofDocTypeToAnswerMapping =
                new Dictionary<string, string[]>(StringComparer.CurrentCultureIgnoreCase)
                {
                    {
                        "Salary", new string[]
                        {
                            FundsDocumentType.Payslip,
                            FundsDocumentType.LetterSalary,
                            FundsDocumentType.AuditedBankStatement,
                            FundsDocumentType.CompanyBankStatement
                        }
                    },
                    {
                        "Dividends / Sale of Company / Company Profits", new string[]
                        {
                            FundsDocumentType.CompanyAccounts,
                            FundsDocumentType.LetterRegulatedAccountant,
                            FundsDocumentType.LetterSolicitor,
                            FundsDocumentType.BusinessSale,
                            FundsDocumentType.DividendContract,
                            FundsDocumentType.CompanyBankStatement
                        }
                    },
                    {
                        "Fiat Investments", new string[]
                        {
                            FundsDocumentType.CashInStatement,
                            FundsDocumentType.InvestmentContractNotes,
                            FundsDocumentType.InvestCertificates,
                            FundsDocumentType.InvestBankStatement,
                            FundsDocumentType.InvestLetterRegulatedAccountant
                        }
                    },
                    {
                        "Cryptocurrency Investments", new string[]
                        {
                            FundsDocumentType.ScreenshotSourceWallet
                        }
                    },
                    {
                        "Retirement Income", new string[]
                        {
                            FundsDocumentType.PensionStatement,
                            FundsDocumentType.PensionLetterRegulatedAccountant,
                            FundsDocumentType.PensionLetterAnnuityProvider,
                            FundsDocumentType.PensionBankStatement,
                            FundsDocumentType.PensionSavingsStatement
                        }
                    },
                    {
                        "Fixed Deposit Savings", new string[]
                        {
                            FundsDocumentType.DepositStatement,
                            FundsDocumentType.DepositEvidence
                        }
                    },
                    {
                        "Gifts / Family / Friends", new string[]
                        {
                            FundsDocumentType.LetterDonor
                        }
                    }
                };

            return answers
                   .SelectMany(GetDocumentTypeByAnswer)
                   .Distinct()
                   .ToArray();

            IEnumerable<string> GetDocumentTypeByAnswer(string answer) =>
                sofDocTypeToAnswerMapping.GetValueOrDefault(answer, new string[] { FundsDocumentType.Other });
        }
    }
}