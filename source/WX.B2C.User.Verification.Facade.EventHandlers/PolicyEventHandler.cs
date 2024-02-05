using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Providers;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Events.EventArgs;
using WX.B2C.User.Verification.Events.Events;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Events.Internal.Extensions;
using WX.Messaging.Subscriber.HandlerResolving;

namespace WX.B2C.User.Verification.Facade.EventHandlers
{
    internal class PolicyEventHandler : BaseEventHandler,
                                        IEventHandler<ApplicationRegisteredEvent>,
                                        IEventHandler<PersonalDetailsUpdatedEvent>
    {
        private readonly IVerificationPolicyStorage _policyStorage;
        private readonly IApplicationStorage _applicationStorage;
        private readonly IApplicationBuilder _applicationBuilder;
        private readonly IApplicationMigrationService _applicationMigrationService;
        private readonly ICountryDetailsProvider _countryDetailsProvider;
        private readonly IFeatureToggleService _featureToggleService;

        public PolicyEventHandler(
            IVerificationPolicyStorage policyStorage,
            IApplicationStorage applicationStorage,
            IApplicationBuilder applicationBuilder,
            IApplicationMigrationService applicationMigrationService,
            EventHandlingContext context,
            ICountryDetailsProvider countryDetailsProvider,
            IFeatureToggleService featureToggleService) : base(context)
        {
            _policyStorage = policyStorage ?? throw new ArgumentNullException(nameof(policyStorage));
            _applicationBuilder = applicationBuilder ?? throw new ArgumentNullException(nameof(applicationBuilder));
            _applicationStorage = applicationStorage ?? throw new ArgumentNullException(nameof(applicationStorage));
            _featureToggleService = featureToggleService ?? throw new ArgumentNullException(nameof(featureToggleService));
            _applicationMigrationService = applicationMigrationService ?? throw new ArgumentNullException(nameof(applicationMigrationService));
            _countryDetailsProvider = countryDetailsProvider ?? throw new ArgumentNullException(nameof(countryDetailsProvider));
        }

        public Task HandleAsync(ApplicationRegisteredEvent @event) =>
            Handle(@event, async args =>
            {
                var policyId = args.PolicyId;
                var applicationId = args.ApplicationId;

                var policy = await _policyStorage.GetAsync(policyId);
                var application = await _applicationStorage.GetAsync(applicationId);

                await _applicationBuilder.BuildAsync(application, policy);
            });

        public Task HandleAsync(PersonalDetailsUpdatedEvent @event) =>
            Handle(@event, async args =>
            {
                const ProductType productType = ProductType.WirexBasic;

                var residenceAddress = args.Changes.Find<AddressDto>(XPathes.ResidenceAddress);
                if (residenceAddress is not { NewValue: not null })
                    return;

                var application = await _applicationStorage.FindAsync(args.UserId, productType);
                if (application is null)
                    return;

                if (application.DecisionReasons.Contains(ApplicationRejectionReasons.Fraud))
                    return;

                var newAddress = residenceAddress.NewValue;
                var region = await _countryDetailsProvider.GetRegionAsync(newAddress.Country);
                var verificationContext = VerificationPolicySelectionContext.Create(newAddress.Country, region, newAddress.State);

                var newPolicyId = await _policyStorage.FindIdAsync(verificationContext);
                if (newPolicyId == application.PolicyId)
                    return;

                var isAvailable = await _featureToggleService.IsVerificationAvailableAsync(verificationContext);
                await _applicationMigrationService.ChangePolicyAsync(application, newPolicyId, isAvailable);
            });
    }
}
