using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Public.Requests;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Mappers
{
    public interface IDocumentMapper
    {
        SubmitDocumentDto Map(SubmitDocumentRequest request, Guid[] fileIds);
    }

    internal class DocumentMapper : IDocumentMapper
    {
        public SubmitDocumentDto Map(SubmitDocumentRequest request, Guid[] fileIds)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (fileIds == null)
                throw new ArgumentNullException(nameof(fileIds));

            return new SubmitDocumentDto
            {
                Category = request.Category,
                Type = request.Type,
                FileIds = fileIds
            };
        }
    }
}