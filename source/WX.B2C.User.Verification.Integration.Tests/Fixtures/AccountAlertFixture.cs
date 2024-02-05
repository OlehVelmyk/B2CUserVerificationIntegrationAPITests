using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using Microsoft.Data.SqlClient;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.Integration.Tests.Builders;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.Core.TypeExtensions;

namespace WX.B2C.User.Verification.Integration.Tests.Fixtures
{
    internal class AccountAlertFixture
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IVerificationDetailsRepository _verificationDetailsRepository;
        private readonly IPersonalDetailsRepository _personalDetailsRepository;
        private readonly IApplicationStateChangelogRepository _changelogRepository;
        private readonly IAppConfig _appConfig;

        public AccountAlertFixture(IApplicationRepository applicationRepository, 
                                   IVerificationDetailsRepository verificationDetailsRepository,
                                   IPersonalDetailsRepository personalDetailsRepository,
                                   IApplicationStateChangelogRepository changelogRepository,
                                   IAppConfig appConfig)
        {
            _applicationRepository = applicationRepository ?? throw new ArgumentNullException(nameof(applicationRepository));
            _verificationDetailsRepository = verificationDetailsRepository ?? throw new ArgumentNullException(nameof(verificationDetailsRepository));
            _personalDetailsRepository = personalDetailsRepository ?? throw new ArgumentNullException(nameof(personalDetailsRepository));
            _changelogRepository = changelogRepository ?? throw new ArgumentNullException(nameof(changelogRepository));
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
        }

        public async Task SaveAsync(IEnumerable<AccountAlertSpecimen> alerts)
        {
            foreach (var alert in alerts)
                await SaveAsync(alert);
        }

        public async Task SaveAsync(AccountAlertSpecimen alert)
        {
            var (applicationSpecimen, verificationDetails, personalDetails, applicationChangelog) = alert;
            applicationSpecimen.RequiredTasks = Array.Empty<VerificationTaskSpecimen>();
            var application = new ApplicationBuilder().From(applicationSpecimen).Build();

            await _applicationRepository.SaveAsync(application);
            await _verificationDetailsRepository.SaveAsync(verificationDetails);
            await _personalDetailsRepository.SaveAsync(personalDetails);
            await _changelogRepository.SaveAsync(applicationChangelog);
        }

        public async Task RemoveAsync(IEnumerable<AccountAlertSpecimen> alerts)
        {
            if (alerts.IsNullOrEmpty())
                return;

            var applicationIds = BuildInSql(alerts.Select(x => x.ApplicationId));
            var userIds = BuildInSql(alerts.Select(x => x.UserId));

            var removeApplicationSql = string.Format("DELETE FROM Applications WHERE Id IN ({0})", applicationIds);
            var removePersonalDetailsSql = string.Format("DELETE FROM PersonalDetails WHERE UserId IN ({0})", applicationIds);
            var removeVerificationDetailsSql = string.Format("DELETE FROM VerificationDetails WHERE UserId IN ({0})", userIds);
            var removeApplicationChangelogSql = string.Format("DELETE FROM ApplicationStateChangelog WHERE ApplicationId IN ({0})", userIds);

            var connectionString = _appConfig.DbConnectionString.UnSecure();
            await using var dbConnection = new SqlConnection(connectionString);

            await dbConnection.ExecuteAsync(removeApplicationChangelogSql, new { ApplicationIds = applicationIds });
            await dbConnection.ExecuteAsync(removeApplicationSql, new { ApplicationIds = applicationIds });
            await dbConnection.ExecuteAsync(removePersonalDetailsSql, new { UserIds = userIds });
            await dbConnection.ExecuteAsync(removeVerificationDetailsSql, new { UserIds = userIds });
        }

        private string BuildInSql(IEnumerable<Guid> ids) =>
            string.Join(",", ids.Select(id => $"'{id}'"));
    }
}
