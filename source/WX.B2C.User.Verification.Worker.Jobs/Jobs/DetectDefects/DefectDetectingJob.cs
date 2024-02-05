using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Validators;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using Task = System.Threading.Tasks.Task;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects
{
    internal class DefectDetectingJob : BatchJob<UserConsistency, DetectDefectJobSettings>
    {
        private readonly UserConsistencyValidator _consistencyValidator;
        private readonly IReportBuilder _reportBuilder;
        private readonly IReportsBlobStorage _reportsBlobStorage;

        public DefectDetectingJob(IUserDefectsModelProvider jobDataProvider,
                                  UserConsistencyValidator consistencyValidator,
                                  IReportBuilder reportBuilder,
                                  IReportsBlobStorage reportsBlobStorage,
                                  ILogger logger) : base(jobDataProvider, logger)
        {
            _consistencyValidator = consistencyValidator ?? throw new ArgumentNullException(nameof(consistencyValidator));
            _reportBuilder = reportBuilder ?? throw new ArgumentNullException(nameof(reportBuilder));
            _reportsBlobStorage = reportsBlobStorage ?? throw new ArgumentNullException(nameof(reportsBlobStorage));
        }

        internal static string Name => "detect-defects";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<DefectDetectingJob>()
                 .WithIdentity(Name, "Validation")
                 .WithDescription("Validate that users are in consistent state in system")
                 .StoreDurably();

        protected override async Task Execute(Batch<UserConsistency> batch,
                                              DetectDefectJobSettings settings,
                                              CancellationToken cancellationToken)
        {
            var reports = await batch.Items.Foreach(DetectDefects);
            var usersWithDefects = reports.Where(report => report.HasDefects);
            await SaveReportsAsync(settings, usersWithDefects);
        }

        protected override async Task FinalizeJob(DetectDefectJobSettings settings, bool isFault)
        {
            if (isFault)
                return;
            if (settings.ReportLinkTtl <= 0)
                return;

            var link = await _reportsBlobStorage.GenerateDownloadHrefAsync(settings.ReportContainer, settings.ReportName, settings.ReportLinkTtl);
            Logger.Information("Report saved. Link:{Link}", link);
        }

        private async Task SaveReportsAsync(DetectDefectJobSettings settings, IEnumerable<Report> usersWithDefects)
        {
            await using var ms = await _reportBuilder.BuildPartAsync(usersWithDefects, settings.IgnoredDefects);
            await _reportsBlobStorage.AppendAsync(settings.ReportContainer, settings.ReportName, ms);
        }

        private Task<Report> DetectDefects(UserConsistency user)
        {
            return Task.Run(() =>
            {
                var result = _consistencyValidator.Validate(user);
                if (result.IsValid)
                    return Report.NoDefects(user.UserId);

                return new Report(user.UserId, result.Errors);
            });
        }
    }
}