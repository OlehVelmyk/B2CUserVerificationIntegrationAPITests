using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Services
{
    public interface IActionTypeMapper
    {
        ActionType? Map(DocumentCategory documentCategory, string documentType);
    }

    internal class ActionTypeMapper : IActionTypeMapper
    {
        public ActionType? Map(DocumentCategory documentCategory, string documentType)
        {
            return (documentCategory, documentType) switch
            {
                (DocumentCategory.ProofOfIdentity, _) => ActionType.ProofOfIdentity,
                (DocumentCategory.ProofOfAddress, _) => ActionType.ProofOfAddress,
                (DocumentCategory.ProofOfFunds, _) => ActionType.ProofOfFunds,
                (DocumentCategory.Selfie, _) => ActionType.Selfie,
                (DocumentCategory.Taxation, _) => ActionType.W9Form,
                _ => null
            };
        }
    }
}