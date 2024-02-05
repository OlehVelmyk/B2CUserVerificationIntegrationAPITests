using System;
using System.Collections.Generic;
using Optional;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Core.Services.Tickets
{
    internal interface ITicketInfoProvider
    {
        public Option<string> FindAsync(string xPath);
    }

    internal class HardCodedTicketInfoProvider : ITicketInfoProvider
    {
        private readonly IReadOnlyDictionary<Guid, string> _surveyTicketInfos = new Dictionary<Guid, string>
        {
            [new Guid("EDDACA4C-C4A6-40C6-8FF3-D63A5D435783")] = TicketReasons.UsaEddCheckListReviewRequired
        };

        private readonly IXPathParser _xPathParser;

        public HardCodedTicketInfoProvider(IXPathParser xPathParser)
        {
            _xPathParser = xPathParser ?? throw new ArgumentNullException(nameof(xPathParser));
        }

        public Option<string> FindAsync(string xPath)
        {
            var xPathDetails = _xPathParser.Parse(xPath);

            return xPathDetails switch
            {
                DocumentsXPathDetails documentsXPathDetails when documentsXPathDetails.Type == TaxationDocumentType.W9Form.Value =>
                    TicketReasons.W9FormReviewRequired.Some(),
                DocumentsXPathDetails { Category: DocumentCategory.ProofOfAddress } =>
                    TicketReasons.ProofOfAddressReviewRequired.Some(),
                DocumentsXPathDetails { Category: DocumentCategory.ProofOfFunds } =>
                    TicketReasons.ProofOfFundsReviewRequired.Some(),
                SurveyXPathDetails surveyXPathDetails => _surveyTicketInfos.Find(surveyXPathDetails.SurveyId),
                _                                     => Option.None<string>()
            };
        }
    }
}