using System;
using System.Collections.Generic;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.CollectionSteps
{
    internal interface ICollectionStepCreatorFactory
    {
        ICollectionStepsCreator Create(string jobName, Func<IEnumerable<string>, IEnumerable<string>> filter, bool useActors);
    }

    internal class CollectionStepCreatorFactory : ICollectionStepCreatorFactory
    {
        private readonly ICollectionStepService _collectionStepService;
        private readonly ICheckProviderService _checkProviderService;
        private readonly ITaskCollectionStepRepository _taskCollectionStepRepository;
        private readonly IVerificationPolicyStorage _policyStorage;
        private readonly ICollectionStepRepository _collectionStepRepository;
        private readonly ILogger _logger;

        public CollectionStepCreatorFactory(ICollectionStepService collectionStepService, 
                                            ICheckProviderService checkProviderService, 
                                            ITaskCollectionStepRepository taskCollectionStepRepository,
                                            IVerificationPolicyStorage policyStorage,
                                            ICollectionStepRepository collectionStepRepository,
                                            ILogger logger)
        {
            _collectionStepService = collectionStepService ?? throw new ArgumentNullException(nameof(collectionStepService));
            _checkProviderService = checkProviderService ?? throw new ArgumentNullException(nameof(checkProviderService));
            _taskCollectionStepRepository = taskCollectionStepRepository ?? throw new ArgumentNullException(nameof(taskCollectionStepRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _policyStorage = policyStorage ?? throw new ArgumentNullException(nameof(policyStorage));
            _collectionStepRepository = collectionStepRepository ?? throw new ArgumentNullException(nameof(collectionStepRepository));
        }

        public ICollectionStepsCreator Create(string jobName, Func<IEnumerable<string>, IEnumerable<string>> filter, bool useActors)
        {
            var policyProvider = new CollectionStepsPolicyProvider(_policyStorage, _checkProviderService);
            ICollectionStepsUpdater updater = useActors
                ? new ActorCollectionStepsUpdater(_collectionStepService, jobName)
                : new DirectCollectionStepsUpdater(_collectionStepRepository);

            return new CollectionStepsCreator(policyProvider,
                                              _collectionStepRepository,
                                              _taskCollectionStepRepository,
                                              updater,
                                              jobName,
                                              filter,
                                              _logger);
        }
    }
}