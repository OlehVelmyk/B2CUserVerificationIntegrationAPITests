using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Onfido.Models
{
    internal class IdentityDocumentsCheckInputData : OnfidoCheckInputData
    {
        public FullNameDto FullName { get; set; }

        public DateTime BirthDate { get; set; }

        public DocumentDto IdentityDocument { get; set; }
    }

    internal class IdentityCheckOutputData : CheckOutputData
    {
        public string Nationality { get; set; }

        public string PoiIssuingCountry { get; set; }

        public string PlaceOfBirth { get; set; }

        public IdDocumentNumberDto IdDocumentNumber { get; set; }

        public string CheckResult { get; set; }

        public string ReportResult { get; set; }

        public object[] Failures { get; set; }
    }
}