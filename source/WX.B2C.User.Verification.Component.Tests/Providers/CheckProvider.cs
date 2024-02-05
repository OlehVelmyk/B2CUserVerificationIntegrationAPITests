using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures;
using WX.B2C.User.Verification.Component.Tests.Helpers;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Component.Tests.Providers
{
    internal interface ICheckProvider
    {
        CheckInfo Get(Guid variantId);

        CheckInfo Get(CheckType type, CheckProviderType provider);

        CollectionStepVariantDto[] GetRequiredData(Guid variantId);
    }

    internal class CheckProvider : ICheckProvider
    {
        private readonly AdminApiClientFactory _adminApiClientFactory;
        private readonly AdministratorFactory _adminFactory;
        private readonly CollectionStepsFixture _stepsFixture;
        private readonly IList<CheckInfo> _checkInfos;
        private readonly StepVariantComparer _stepVariantComparer;

        public CheckProvider(AdminApiClientFactory adminApiClientFactory,
                             AdministratorFactory adminFactory,
                             CollectionStepsFixture stepsFixture,
                             IOptions<IList<CheckInfo>> checkInfosOptions, 
                             StepVariantComparer stepVariantComparer)
        {
            _adminApiClientFactory = adminApiClientFactory ?? throw new ArgumentNullException(nameof(adminApiClientFactory));
            _adminFactory = adminFactory ?? throw new ArgumentNullException(nameof(adminFactory));
            _stepsFixture = stepsFixture ?? throw new ArgumentNullException(nameof(stepsFixture));
            _checkInfos = checkInfosOptions?.Value ?? throw new ArgumentNullException(nameof(checkInfosOptions));
            _stepVariantComparer = stepVariantComparer ?? throw new ArgumentNullException(nameof(stepVariantComparer));
        }

        public CheckInfo Get(Guid variantId) =>
            _checkInfos.First(info => info.VariantId == variantId);

        public CheckInfo Get(CheckType type, CheckProviderType provider) =>
            _checkInfos.First(info => info.Type == type && info.Provider == provider);

        public CollectionStepVariantDto[] GetRequiredData(Guid variantId) =>
            Get(variantId).RequiredData;
    }
}
