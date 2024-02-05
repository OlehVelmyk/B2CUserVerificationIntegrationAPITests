using System;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Services
{
    public interface IDocumentCategoryMapper
    {
        DocumentCategory Map(ActionType actionType);
    }
    
    internal class DocumentCategoryMapper: IDocumentCategoryMapper
    {
        /// <summary>
        /// Reflect IActionTypeMapper.Map()
        /// </summary>
        public DocumentCategory Map(ActionType actionType)
        {
            return actionType switch
            {
                ActionType.ProofOfIdentity => DocumentCategory.ProofOfIdentity,
                ActionType.ProofOfAddress => DocumentCategory.ProofOfAddress,
                ActionType.ProofOfFunds => DocumentCategory.ProofOfFunds,
                ActionType.Selfie => DocumentCategory.Selfie,
                ActionType.W9Form => DocumentCategory.Taxation,
                _ => throw new ArgumentOutOfRangeException(nameof(actionType), actionType, "Unsupported action type.")
            };
        }
    }    
}

