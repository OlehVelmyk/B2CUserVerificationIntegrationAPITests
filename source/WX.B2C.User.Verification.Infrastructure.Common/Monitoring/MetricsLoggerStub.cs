using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Infrastructure.Common.Monitoring
{
    internal class MetricsLoggerStub : IMetricsLogger
    {
        public Task<T> ExecuteWithServiceDependencyTracking<T>(string operationProtocol,
                                                               string operationTargetType,
                                                               string operationName,
                                                               Func<Task<T>> action,
                                                               bool throttle = false) =>
            action();

        public Task ExecuteWithServiceDependencyTracking(string operationProtocol,
                                                         string operationTargetType,
                                                         string operationName,
                                                         Func<Task> action,
                                                         bool throttle = false) =>
            action();

        public Task ExecuteWithCacheDependencyTracking(string dependencyType, string dependencyName, Func<Task> action) =>
            action();

        public Task<T> ExecuteWithCacheDependencyTracking<T>(string dependencyType, string dependencyName, Func<Task<T>> action) =>
            action();

        public Task<T> ExecuteWithStorageDependencyTracking<T>(string dependencyType, string dependencyName, Func<Task<T>> action) =>
            action();

        public Task ExecuteWithStorageDependencyTracking(string dependencyType, string dependencyName, Func<Task> action) =>
            action();

        public Task ExecuteWithDatabaseDependencyTracking(string dependencyType, string dependencyName, Func<Task> action) =>
            action();

        public Task<T> ExecuteWithDatabaseDependencyTracking<T>(string dependencyType, string dependencyName, Func<Task<T>> action) =>
            action();

        public Task<T> ExecuteWithRpcRequestTracking<T>(Func<Task<T>> action) =>
            action();

        public Task ExecuteWithRpcRequestTracking(Func<Task> action, string operationName = null) =>
            action();

        public Task<T> ExecuteWithEventHandlingTracking<T>(Func<Task<T>> action) =>
            action();

        public Task ExecuteWithEventHandlingTracking(Func<Task> action) =>
            action();

        public Task<T> ExecuteWithCommandHandlingTracking<T>(Func<Task<T>> action) =>
            action();

        public Task ExecuteWithCommandHandlingTracking(Func<Task> action) =>
            action();

        public Task<T> ExecuteWithBusinessLogicTracking<T>(string operationTargetType, string operationName, Func<Task<T>> action) =>
            action();

        public Task ExecuteWithBusinessLogicTracking(string operationTargetType, string operationName, Func<Task> action) =>
            action();

        public Task ExecuteWithHttpRequestTracking(Func<Task> action, 
                                                   Func<int> getResultStatus, 
                                                   Func<Exception> getException) =>
            action();

        public Task<T> ExecuteWithHttpRequestTracking<T>(Func<Task<T>> action, 
                                                         Func<int> getResultStatus, 
                                                         Func<Exception> getException) =>
            action();
    }
}