using System;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.PassFort.Mappers
{
    public interface IPassFortEnumerationsMapper
    {
        (Client.Models.DocumentCategory, Client.Models.DocumentType) Map(DocumentCategory category, string type);

        string Map(ApplicationState state);
        
        ApplicationState Map(string state);

        Client.Models.TaskType Map(TaskType type);

        TaskType Map(Client.Models.TaskType? type);

        Client.Models.DocumentType MapIdentityType(string documentType);
    }

    internal class PassFortEnumerationsMapper : IPassFortEnumerationsMapper
    {
        public (Client.Models.DocumentCategory, Client.Models.DocumentType) Map(DocumentCategory category, string type) =>
            category switch
            {
                DocumentCategory.ProofOfIdentity => (Client.Models.DocumentCategory.PROOFOFIDENTITY, MapIdentityType(type)),
                DocumentCategory.ProofOfAddress => (Client.Models.DocumentCategory.PROOFOFADDRESS, MapAddressType(type)),
                DocumentCategory.ProofOfFunds => (Client.Models.DocumentCategory.PROOFOFSOURCEOFFUNDS, Client.Models.DocumentType.UNKNOWN),
                DocumentCategory.Taxation => (Client.Models.DocumentCategory.PROOFOFTAXSTATUS, Client.Models.DocumentType.UNKNOWN),
                DocumentCategory.Supporting => (Client.Models.DocumentCategory.SUPPORTING, Client.Models.DocumentType.UNKNOWN),
                _ => throw new ArgumentOutOfRangeException(nameof(category), category, "Unsupported document category.")
            };

        public string Map(ApplicationState state) =>
            state switch
            {
                ApplicationState.Applied   => Constants.ApplicationState.Applied,
                ApplicationState.Approved  => Constants.ApplicationState.Approved,
                ApplicationState.Cancelled => Constants.ApplicationState.Cancelled,
                ApplicationState.Rejected  => Constants.ApplicationState.Rejected,
                ApplicationState.InReview  => Constants.ApplicationState.InReview,
                _                          => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };

        public ApplicationState Map(string state) =>
            state switch
            {
                Constants.ApplicationState.Applied   => ApplicationState.Applied,
                Constants.ApplicationState.Approved  => ApplicationState.Approved,
                Constants.ApplicationState.Cancelled => ApplicationState.Cancelled,
                Constants.ApplicationState.Rejected  => ApplicationState.Rejected,
                Constants.ApplicationState.InReview  => ApplicationState.InReview,
                _                                     => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };

        public Client.Models.TaskType Map(TaskType type) =>
            type switch
            {
                TaskType.Address            => Client.Models.TaskType.INDIVIDUALVERIFYADDRESS,
                TaskType.Identity           => Client.Models.TaskType.INDIVIDUALVERIFYIDENTITY,
                TaskType.ProofOfFunds       => Client.Models.TaskType.INDIVIDUALVERIFYSOURCEOFFUNDS,
                TaskType.FinancialCondition => Client.Models.TaskType.INDIVIDUALVERIFYBANKACCOUNT,
                TaskType.PepScreening       => Client.Models.TaskType.INDIVIDUALASSESSPOLITICALEXPOSURE,
                TaskType.FraudScreening     => Client.Models.TaskType.INDIVIDUALFRAUDSCREENING,
                TaskType.TaxResidence       => Client.Models.TaskType.INDIVIDUALVERIFYTAXSTATUS,
                TaskType.RiskListsScreening => Client.Models.TaskType.INDIVIDUALASSESSMEDIAANDPOLITICALANDSANCTIONSEXPOSURE,
                _                           => Client.Models.TaskType.INDIVIDUALMANUALTASK
            };

        public TaskType Map(Client.Models.TaskType? type)
        {
            if (type == Client.Models.TaskType.INDIVIDUALVERIFYADDRESS) return TaskType.Address;
            if (type == Client.Models.TaskType.INDIVIDUALVERIFYIDENTITY) return TaskType.Identity;
            if (type == Client.Models.TaskType.INDIVIDUALVERIFYSOURCEOFFUNDS) return TaskType.ProofOfFunds;
            if (type == Client.Models.TaskType.INDIVIDUALVERIFYBANKACCOUNT) return TaskType.FinancialCondition;
            if (type == Client.Models.TaskType.INDIVIDUALASSESSPOLITICALEXPOSURE) return TaskType.PepScreening;
            if (type == Client.Models.TaskType.INDIVIDUALFRAUDSCREENING) return TaskType.FraudScreening;
            if (type == Client.Models.TaskType.INDIVIDUALVERIFYTAXSTATUS) return TaskType.TaxResidence;
            if (type == Client.Models.TaskType.INDIVIDUALASSESSMEDIAANDPOLITICALANDSANCTIONSEXPOSURE) return TaskType.RiskListsScreening;

            throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported task type.");
        }


        public Client.Models.DocumentType MapIdentityType(string type) =>
            type switch
            {
                nameof(IdentityDocumentType.Passport) => Client.Models.DocumentType.PASSPORT,
                nameof(IdentityDocumentType.IdentityCard) => Client.Models.DocumentType.STATEID,
                nameof(IdentityDocumentType.DriverLicense) => Client.Models.DocumentType.DRIVINGLICENCE,
                nameof(IdentityDocumentType.BirthCertificate) => Client.Models.DocumentType.BIRTHCERTIFICATE,
                nameof(IdentityDocumentType.Other) => Client.Models.DocumentType.UNKNOWN,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported identity document type.")
            };

        private static Client.Models.DocumentType MapAddressType(string type) =>
            type switch
            {
                nameof(AddressDocumentType.BankStatement) => Client.Models.DocumentType.BANKSTATEMENT,
                nameof(AddressDocumentType.UtilityBill) => Client.Models.DocumentType.UTILITYBILL,
                nameof(AddressDocumentType.TaxReturn) => Client.Models.DocumentType.ANNUALRETURN,
                nameof(AddressDocumentType.CouncilTax) => Client.Models.DocumentType.UNKNOWN,
                nameof(AddressDocumentType.CertificateOfResidency) => Client.Models.DocumentType.UNKNOWN,
                nameof(AddressDocumentType.Other) => Client.Models.DocumentType.UNKNOWN,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported address document type.")
            };
    }
}
