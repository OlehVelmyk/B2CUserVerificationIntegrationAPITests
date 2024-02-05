using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.UserEmails;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Core.Services.UserEmails
{
    /// <summary>
    /// TODO PHASE 2 Reimplement UserEmailService.
    /// Make more generic socultion
    /// Reduce code duplication
    /// Make templates configurable
    /// Remove hardcoded template names
    /// </summary>
    internal class UserEmailService : IUserEmailService
    {
        private const string NoreplyEmail = nameof(NoreplyEmail);

        private readonly IUserEmailProvider _userEmailProvider;
        private readonly IProfileStorage _profileStorage;
        private readonly ICollectionStepStorage _collectionStepStorage;
        private readonly IXPathParser _xpathParser;
        private readonly IHostSettingsProvider _hostSettingsProvider;
        private readonly IApplicationStorage _applicationStorage;
        private readonly IParametersProvider _parametersProvider;

        public UserEmailService(IUserEmailProvider userEmailProvider,
                                IProfileStorage profileStorage,
                                ICollectionStepStorage collectionStepStorage,
                                IXPathParser xpathParser,
                                IHostSettingsProvider hostSettingsProvider,
                                IApplicationStorage applicationStorage,
                                IParametersProvider parametersProvider)
        {
            _userEmailProvider = userEmailProvider ?? throw new ArgumentNullException(nameof(userEmailProvider));
            _profileStorage = profileStorage ?? throw new ArgumentNullException(nameof(profileStorage));
            _collectionStepStorage = collectionStepStorage ?? throw new ArgumentNullException(nameof(collectionStepStorage));
            _xpathParser = xpathParser ?? throw new ArgumentNullException(nameof(xpathParser));
            _hostSettingsProvider = hostSettingsProvider ?? throw new ArgumentNullException(nameof(hostSettingsProvider));
            _applicationStorage = applicationStorage ?? throw new ArgumentNullException(nameof(applicationStorage));
            _parametersProvider = parametersProvider ?? throw new ArgumentNullException(nameof(parametersProvider));
        }

        public Task SendAsync(ApplicationStateChangedEmailContext context)
        {
            var emailTemplate = context.NewState switch
            {
                ApplicationState.Approved  => EmailSendTemplates.UserVerificationApproved,
                ApplicationState.Cancelled => EmailSendTemplates.UserVerificationApplicationRejected,
                ApplicationState.Rejected  => EmailSendTemplates.UserVerificationApplicationRejected,
                _                          => null
            };

            return emailTemplate != null 
                ? SendAsync(context.UserId, emailTemplate)
                : Task.CompletedTask;
        }

        public async Task SendAsync(CollectionStepRequestedEmailContext context)
        {
            var xpath = _xpathParser.Parse(context.XPath);

            if (xpath is not DocumentsXPathDetails details)
                return;

            var steps = await _collectionStepStorage.GetAllAsync(context.UserId, details.XPath);
            var emailTemplate = FindDocumentEmailTemplate(steps, details);
            if (emailTemplate == null)
                return;

            await SendAsync(context.UserId, emailTemplate, EmailParametersContext.Create().With(context.Reason));
        }

        public async Task SendAsync(ReminderSentEmailContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var applicationState = await _applicationStorage.GetStateAsync(context.UserId, ProductType.WirexBasic);
            var emailParametersContext = EmailParametersContext.Create().With(applicationState);

            await SendAsync(context.UserId, EmailSendTemplates.UserVerificationReminder, emailParametersContext);
        }

        private static string FindDocumentEmailTemplate(CollectionStepDto[] steps, DocumentsXPathDetails details)
        {
            var isFirstTimeRequested = steps.Length == 1;
            //TODO Open question: If requested second time we assume that first time documents was rejected. Why trigger is new collection step and not reviewed - rejected?
            //TODO Open question: do we need to send if requested second time proof of identity or selfie?
            if (!isFirstTimeRequested)
                return EmailSendTemplates.UserVerificationRejected;

            return details switch
            {
                { Category: DocumentCategory.ProofOfFunds }   => EmailSendTemplates.AdditionalDocsRequiredFunds,
                { Category: DocumentCategory.ProofOfAddress } => EmailSendTemplates.AdditionalDocsRequired,
                { Category: DocumentCategory.Taxation } when details.Type == TaxationDocumentType.W9Form.Value => EmailSendTemplates
                    .W9FormRequired,
                _ => null
            };
        }

        private async Task SendAsync(Guid userId, string template, EmailParametersContext parametersContext = null)
        {
            if (!await IsAutomatedAsync(userId))
                return;

            if (parametersContext == null)
                parametersContext = EmailParametersContext.Create();

            var personalDetails = await _profileStorage.GetPersonalDetailsAsync(userId);
            parametersContext.With(personalDetails);
            var email = personalDetails.Email;
            var fromEmail = _hostSettingsProvider.GetSetting(NoreplyEmail);
            var parameters = _parametersProvider.GetParameters(template, parametersContext);

            var emailParameters = new SendEmailParameters
            {
                Emails = new[] { email },
                TemplateId = template,
                FromName = fromEmail,
                FromEmail = fromEmail,
                TemplateParameters = parameters
            };

            await _userEmailProvider.SendEmailAsync(emailParameters);
        }

        /// <summary>
        /// TODO WRXB-10546 remove before final release. Temporary remove functionality for sending email when user is automated false
        /// </summary>
        private Task<bool> IsAutomatedAsync(Guid userId) =>
            _applicationStorage.IsAutomatedAsync(userId, ProductType.WirexBasic);
    }
}