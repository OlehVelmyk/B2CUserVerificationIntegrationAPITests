using System;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Storages;

namespace WX.B2C.User.Verification.Core.Services.RequiredData
{
    public class ProfileProviderFactory : IProfileProviderFactory
    {
        private readonly IProfileStorage _profileStorage;
        private readonly ICollectionStepStorage _collectionStepStorage;
        private readonly IDocumentStorage _documentStorage;
        private readonly IXPathParser _xPathParser;

        public ProfileProviderFactory(IProfileStorage profileStorage,
                                      ICollectionStepStorage collectionStepStorage,
                                      IDocumentStorage documentStorage, 
                                      IXPathParser xPathParser)
        {
            _profileStorage = profileStorage ?? throw new ArgumentNullException(nameof(profileStorage));
            _collectionStepStorage = collectionStepStorage ?? throw new ArgumentNullException(nameof(collectionStepStorage));
            _documentStorage = documentStorage ?? throw new ArgumentNullException(nameof(documentStorage));
            _xPathParser = xPathParser ?? throw new ArgumentNullException(nameof(xPathParser));
        }

        public IProfileDataProvider Create(Guid userId) =>
            new ProfileDataProvider(userId, 
                                    _collectionStepStorage, 
                                    _profileStorage, 
                                    _documentStorage, 
                                    _xPathParser);
    }
}