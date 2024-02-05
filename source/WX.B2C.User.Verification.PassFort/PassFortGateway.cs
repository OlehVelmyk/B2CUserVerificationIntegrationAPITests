using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Serilog;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.PassFort.Client;
using WX.B2C.User.Verification.PassFort.Client.Models;
using WX.B2C.User.Verification.PassFort.Exceptions;
using WX.B2C.User.Verification.PassFort.Mappers;
using DocumentCategory = WX.B2C.User.Verification.Domain.Enums.DocumentCategory;
using TaskType = WX.B2C.User.Verification.Domain.Models.TaskType;

namespace WX.B2C.User.Verification.PassFort
{
    public class PassFortApplication
    {
        public string Id { get; set; }

        public ApplicationState State { get; set; }

        public string ProductId { get; set; }

        public bool IsApproveBlocked { get; set; }

        public TaskType[] RequiredTasks { get; set; }
    }

    public class PassFortTask
    {
        public string Id { get; set; }

        public bool? IsExpired { get; set; }
    }

    public interface IPassFortGateway
    {
        Task<PassFortApplication> GetApplicationAsync(string profileId, string productId);

        Task<PassFortApplication> FindApplicationAsync(string profileId, string productId);

        Task CreateApplicationAsync(string profileId, string productId);

        Task UpdateApplicationStateAsync(string profileId, string applicationId, ApplicationState state);

        Task RevertApplicationDecisionAsync(string profileId, string applicationId);

        Task UpdateTaskStateAsync(string profileId, string taskId, TaskResult? taskResult);

        Task<PassFortTask> GetTaskAsync(string profileId, TaskType type);

        Task<PassFortTask> CreateTaskAsync(string profileId, TaskType taskType);

        Task UploadDocumentAsync(string profileId, DocumentCategory category, string type, IEnumerable<string> imageIds);

        Task<string> UploadDocumentImageAsync(string profileId, Stream data);
    }

    internal class PassFortGateway : BasePassFortGateway, IPassFortGateway
    {
        private readonly IPassFortApiClientFactory _clientFactory;
        private readonly IPassFortEnumerationsMapper _enumerationsMapper;
        private readonly IPassFortApplicationMapper _applicationMapper;

        public PassFortGateway(
            IPassFortApiClientFactory clientFactory,
            IPassFortEnumerationsMapper enumerationsMapper,
            IPassFortApplicationMapper applicationMapper,
            ILogger logger)
            : base(logger)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _enumerationsMapper = enumerationsMapper ?? throw new ArgumentNullException(nameof(enumerationsMapper));
            _applicationMapper = applicationMapper ?? throw new ArgumentNullException(nameof(applicationMapper));
        }

        public async Task<PassFortApplication> GetApplicationAsync(string profileId, string productId)
        {
            var application = await FindApplicationAsync(profileId, productId);
            return application ?? throw new PassFortEntityNotFoundException(profileId, nameof(Application));
        }

        public async Task<PassFortApplication> FindApplicationAsync(string profileId, string productId)
        {
            using var client = _clientFactory.Create();
            var application = await HandleAsync(requestFactory: () => profileId,
                requestInvoker: client.Applications.ListAsync,
                responseMapper: applications =>
                {
                    var application = applications.FirstOrDefault(app => app.Product.Id == productId);
                    return application == null ? null : _applicationMapper.Map(application);
                });

            return application;
        }

        public async Task CreateApplicationAsync(string profileId, string productId)
        {
            using var client = _clientFactory.Create();
            await HandleAsync(
                requestFactory: () => (profileId, new ProductApplication { Product = new ProductResource { Id = productId } }),
                requestInvoker: client.Applications.CreateAsync);
        }

        public async Task UpdateApplicationStateAsync(string profileId, string applicationId, ApplicationState state)
        {
            var application = new ProductApplication { Status = _enumerationsMapper.Map(state) };

            using var client = _clientFactory.Create();
            _ = await HandleAsync(
                requestFactory: () => (profileId, applicationId, application),
                requestInvoker: client.Applications.UpdateAsync);
        }

        public async Task RevertApplicationDecisionAsync(string profileId, string applicationId)
        {
            var revertDecision = new ProductApplicationRevertDecision();

            using var client = _clientFactory.Create();
            _ = await HandleAsync(
                requestFactory: () => (profileId, applicationId, revertDecision),
                requestInvoker: client.Applications.RevertDecisionAsync);
        }

        public async Task<PassFortTask> GetTaskAsync(string profileId, TaskType type)
        {
            var taskType = _enumerationsMapper.Map(type);

            using var client = _clientFactory.Create();
            return await HandleAsync(
                requestFactory: () => profileId,
                requestInvoker: client.Tasks.ListAsync,
                responseMapper: tasks =>
                {
                    var task = tasks
                               .Where(task => task.Type == taskType)
                               .OrderByDescending(task => task.IsExpired)
                               .FirstOrDefault();

                    return task == null ? null : new PassFortTask
                    {
                        Id = task.Id,
                        IsExpired = task.IsExpired
                    };
                });
        }

        public async Task<PassFortTask> CreateTaskAsync(string profileId, TaskType taskType)
        {
            var taskRequest = new TaskRequest
            {
                Type = _enumerationsMapper.Map(taskType),
            };

            using var client = _clientFactory.Create();
            return await HandleAsync(
                requestFactory: () => (profileId, taskRequest),
                requestInvoker: client.Tasks.AddAsync,
                responseMapper: task => new PassFortTask
                {
                    Id = task.Id,
                    IsExpired = task.IsExpired
                });
        }

        public async Task UpdateTaskStateAsync(string profileId, string taskId, TaskResult? taskResult)
        {
            var passfortTaskState = taskResult switch
            {
                TaskResult.Passed => Client.Models.TaskState.COMPLETEDPASS,
                TaskResult.Failed => Client.Models.TaskState.COMPLETEDFAIL,
                null => Client.Models.TaskState.INCOMPLETE,
                _ => throw new ArgumentOutOfRangeException(nameof(taskResult), taskResult, null)
            };

            var task = new TaskResource
            {
                State = passfortTaskState
            };

            using var client = _clientFactory.Create();
            _ = await HandleAsync(
                requestFactory: () => (profileId, taskId, task),
                requestInvoker: client.Tasks.CompleteAsync);
        }

        public async Task UploadDocumentAsync(string profileId, DocumentCategory category, string type, IEnumerable<string> imageIds)
        {
            var (documentCategory, documentType) = _enumerationsMapper.Map(category, type);
            var images = imageIds.Select(imageId => new DocumentPostImagesItem(imageId)).ToArray();

            var newDocument = new DocumentPost
            {
                Category = documentCategory,
                DocumentType = documentType,
                Images = images
            };

            using var client = _clientFactory.Create();
            _ = await HandleAsync(
                requestFactory: () => (profileId, newDocument),
                requestInvoker: client.Profiles.AddDocumentsAsync);
        }

        public async Task<string> UploadDocumentImageAsync(string profileId, Stream data)
        {
            var base64Stream = new CryptoStream(data, new ToBase64Transform(), CryptoStreamMode.Read);
            var streamReader = new StreamReader(base64Stream);
            var base64EncodedData = await streamReader.ReadToEndAsync();

            var newDocumentImage = new DocumentImageResource
            {
                Data = base64EncodedData,
                ImageType = DocumentImageType.FRONT
            };

            using var client = _clientFactory.Create();
            return await HandleAsync(
                requestFactory: () => (profileId, newDocumentImage),
                requestInvoker: client.Documents.UploadAsync,
                responseMapper: documentImage => documentImage.Id
            );
        }
    }
}