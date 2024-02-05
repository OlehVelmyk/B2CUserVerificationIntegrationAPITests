using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Services.UserEmails
{
    internal class EmailParametersContext
    {
        public static EmailParametersContext Create() => new();

        public PersonalDetailsDto PersonalDetails { get; private set; }

        public string Reason { get; private set; }

        public ApplicationState ApplicationState { get; private set; }

        public EmailParametersContext With(PersonalDetailsDto personalDetails)
        {
            PersonalDetails = personalDetails;
            return this;
        }

        public EmailParametersContext With(string reason)
        {
            Reason = reason;
            return this;
        }

        public EmailParametersContext With(ApplicationState applicationState)
        {
            ApplicationState = applicationState;
            return this;
        }
    }

    internal interface IParametersProvider
    {
        Dictionary<string, string> GetParameters(string template, EmailParametersContext context);
    }

    internal class ParametersProvider : IParametersProvider
    {
        private delegate (string key, string value) ExtractParametersDelegate(EmailParametersContext context);

        private readonly Dictionary<string, ExtractParametersDelegate[]> _templateExtractors = new();

        public ParametersProvider()
        {
            Register(EmailSendTemplates.UserVerificationRejected, FirstName, RejectReason);
            Register(EmailSendTemplates.AdditionalDocsRequired, FirstName);
            Register(EmailSendTemplates.UserVerificationApproved, FirstName);
            Register(EmailSendTemplates.W9FormRequired, FirstName);
            Register(EmailSendTemplates.AdditionalDocsRequiredFunds, FirstName);
            Register(EmailSendTemplates.UserVerificationApplicationRejected, FirstName);
            Register(EmailSendTemplates.UserVerificationReminder, FirstName, ApplicationState);
        }

        public Dictionary<string, string> GetParameters(string template, EmailParametersContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!_templateExtractors.TryGetValue(template, out var extractors))
                return null;

            var parameters = new Dictionary<string, string>();
            foreach (var extractor in extractors)
            {
                var (key, value) = extractor(context);
                parameters.Add(key, value);
            }
            return parameters;
        }

        private void Register(string template, params ExtractParametersDelegate[] convertors) =>
            _templateExtractors[template] = convertors;

        private (string, string) RejectReason(EmailParametersContext context) =>
            ("REJECT_REASON", context.Reason);

        private (string, string) FirstName(EmailParametersContext context) =>
            ("FName", context.PersonalDetails.FirstName);

        private (string, string) ApplicationState(EmailParametersContext context) =>
            ("AState", context.ApplicationState.ToString());
    }
}