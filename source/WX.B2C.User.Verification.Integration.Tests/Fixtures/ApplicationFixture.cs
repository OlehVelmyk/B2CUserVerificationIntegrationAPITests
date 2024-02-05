using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Builders;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Fixtures
{
    internal class ApplicationFixture
    {
        private readonly DbFixture _dbFixture;
        private readonly IApplicationRepository _applicationRepository;

        public ApplicationFixture(DbFixture dbFixture, IApplicationRepository applicationRepository)
        {
            _dbFixture = dbFixture ?? throw new ArgumentNullException(nameof(dbFixture));
            _applicationRepository = applicationRepository ?? throw new ArgumentNullException(nameof(applicationRepository));
        }

        public async Task SaveRelationsAsync(ApplicationSpecimen appSpecimen)
        {
            await appSpecimen.RequiredTasks.Select(t => Map(t, appSpecimen.UserId))
                             .Foreach(c => _dbFixture.DbContext.AddAsync(c).AsTask());
            await _dbFixture.DbContext.SaveChangesAsync();
        }

        private Verification.DataAccess.Entities.VerificationTask Map(VerificationTaskSpecimen task, Guid userId) =>
            new Verification.DataAccess.Entities.VerificationTask
            {
                Id = task.Id,
                State = task.State,
                UserId = userId,
                VariantId = task.VariantId,
                IsExpired = default,
                Type = task.Type,
                Result = task.Result,
                ExpirationReason = task.ExpirationDetails?.ExpirationReason,
                ExpiredAt = task.ExpirationDetails?.ExpiredAt
            };

        public async Task SaveAsync(ApplicationSpecimen applicationSpecimen)
        {
            await SaveRelationsAsync(applicationSpecimen);

            var application = new ApplicationBuilder().From(applicationSpecimen).Build();
            await _applicationRepository.SaveAsync(application);
        }
    }
}
