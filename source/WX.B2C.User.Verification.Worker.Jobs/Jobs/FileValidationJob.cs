using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Worker.Jobs.Clients;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Providers;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs
{
    internal class FileValidationJob : BatchJob<FileData, FileValidationJobSettings>
    {
        private readonly IFileBlobStorage _blobStorage;
        private readonly IThrottledOnfidoApiClientFactory _onfidoApiClientFactory;

        public FileValidationJob(IFileBlobStorage blobStorage,
                                 IThrottledOnfidoApiClientFactory onfidoApiClientFactory,
                                 IFileDataProvider jobDataProvider,
                                 ILogger logger) : base(jobDataProvider, logger)
        {
            _blobStorage = blobStorage ?? throw new ArgumentNullException(nameof(blobStorage));
            _onfidoApiClientFactory = onfidoApiClientFactory ?? throw new ArgumentNullException(nameof(onfidoApiClientFactory));
        }

        public static string Name => "file-validation-trigger";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<FileValidationJob>()
                 .WithIdentity(Name, "Backfill")
                 .WithDescription("Validate imported files");

        protected override async Task Execute(Batch<FileData> batch, FileValidationJobSettings settings, CancellationToken cancellationToken)
        {
            var client = _onfidoApiClientFactory.Create(settings.OnfidoRequestPerMinute);

            foreach (var file in batch.Items)
            {
                var context = new FileValidationContext(file);
                await Task.WhenAll(ValidateExternalIdAsync(context, client),
                                   ValidateBlobStorageAsync(context),
                                   ValidateProviderAsync(context));

                if (context.HasFailure)
                {
                    Logger
                        .ForContext(nameof(FileData.UserId), file.UserId)
                        .ForContext(nameof(FileData.Id), file.Id)
                        .ForContext(nameof(FileData.Provider), file.Provider)
                        .ForContext(nameof(FileData.FileName), file.FileName)
                        .Information("File validation failed: {Failures}", context.Failures);
                }
            }
        }

        private async Task ValidateExternalIdAsync(FileValidationContext context, IOnfidoApiClientWithThrottling client)
        {
            if (context.File.ExternalId == null || context.File.Provider == ExternalFileProviderType.Onfido)
                return;
            
            try
            {
                await client.FindDocumentAsync(context.File.ExternalId);
            }
            catch (Exception e) when (e.Message.Contains("NotFound"))
            {
                context.AddFailure($"File not found in Onfido by external id {context.File.ExternalId}");
            }
        }

        private Task ValidateProviderAsync(FileValidationContext context)
        {
            if (context.File.Provider.HasValue && context.File.ExternalId is null)
                context.AddFailure("External id is absent");

            return Task.CompletedTask;
        }

        private async Task ValidateBlobStorageAsync(FileValidationContext context)
        {
            var file = context.File;
            var isExists = await _blobStorage.ExistsAsync(file.UserId, file.Id, file.FileName);
            if (!isExists)
                context.AddFailure("File does not exists in blob");
        }

        private class FileValidationContext
        {
            private readonly ConcurrentBag<string> _failures = new();

            public FileData File { get; }

            public IReadOnlyCollection<string> Failures => _failures;

            public bool HasFailure => !_failures.IsEmpty;

            public FileValidationContext(FileData file)
            {
                File = file ?? throw new ArgumentNullException(nameof(file));
            }

            public void AddFailure(string error) => _failures.Add(error);
        }
    }
}