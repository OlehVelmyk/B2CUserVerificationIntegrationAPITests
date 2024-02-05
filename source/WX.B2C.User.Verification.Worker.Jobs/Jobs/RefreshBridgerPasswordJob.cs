using System;
using System.Threading.Tasks;
using Quartz;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Worker.Jobs.Services;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs
{
    internal class BridgerPasswordRefreshSetting
    {
        public string UserId { get; set; }

        public int DaysBeforeChangePassword { get; set; }
    }

    internal sealed class RefreshBridgerPasswordJob : IJob
    {
        private readonly BridgerPasswordRefreshSetting _setting;
        private readonly IBridgerCredentialsService _credentialsService;
        private readonly IBridgerPasswordGenerator _passwordGenerator;
        private readonly ILogger _logger;

        public RefreshBridgerPasswordJob(
            BridgerPasswordRefreshSetting setting,
            IBridgerCredentialsService credentialsService,
            IBridgerPasswordGenerator passwordGenerator,
            ILogger logger)
        {
            _setting = setting ?? throw new ArgumentNullException(nameof(setting));
            _credentialsService = credentialsService ?? throw new ArgumentNullException(nameof(credentialsService));
            _passwordGenerator = passwordGenerator ?? throw new ArgumentNullException(nameof(passwordGenerator));
            _logger = logger?.ForContext<RefreshBridgerPasswordJob>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public static string Name => "refresh-bridger-password";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<RefreshBridgerPasswordJob>()
                 .WithIdentity(Name, "Important")
                 .WithDescription("Updates the password in LexisNexis Bridger before it expires.")
                 .StoreDurably()
                 .RequestRecovery();

        public async Task Execute(IJobExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            try
            {
                var daysBeforePasswordExpires = await _credentialsService.GetDaysUntilPasswordExpiresAsync(_setting.UserId);
                var daysBeforePasswordChange = daysBeforePasswordExpires - _setting.DaysBeforeChangePassword;
                if (daysBeforePasswordChange <= 0)
                    await UpdatePasswordAsync(_setting.UserId);
                else
                    _logger
                        .ForContext(nameof(daysBeforePasswordExpires), daysBeforePasswordExpires)
                        .Information("LexisNexis Bridger password will be changed in {DaysBeforePasswordChange}", daysBeforePasswordChange);
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Could not update password in LexisNexis Bridger.");
                throw new JobExecutionException(exc);
            }
        }

        private async Task UpdatePasswordAsync(string userId)
        {
            var newPassword = _passwordGenerator.Generate();
            await _credentialsService.UpdateAsync(userId, newPassword);
        }
    }
}
