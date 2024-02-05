using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures
{
    internal abstract class CollectionStepFixture<T> where T : CollectionStepVariantDto
    {
        public abstract Task CompleteAsync(Guid userId, T variant, Seed seed);

        public abstract Task MoveInReviewAsync(Guid userId, T variant, Seed seed);
    }
}
