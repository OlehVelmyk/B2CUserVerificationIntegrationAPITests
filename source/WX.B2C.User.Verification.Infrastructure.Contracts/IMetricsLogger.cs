using System;
using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Infrastructure.Contracts
{
    public interface IMetricsLogger
    {
        Task<T> ExecuteWithServiceDependencyTracking<T>(string operationProtocol,
                                                        string operationTargetType,
                                                        string operationName,
                                                        Func<Task<T>> action,
                                                        bool throttle = false);

        Task ExecuteWithServiceDependencyTracking(string operationProtocol,
                                                  string operationTargetType,
                                                  string operationName,
                                                  Func<Task> action,
                                                  bool throttle = false);

        Task ExecuteWithCacheDependencyTracking(string dependencyType, string dependencyName, Func<Task> action);

        Task<T> ExecuteWithCacheDependencyTracking<T>(string dependencyType, string dependencyName, Func<Task<T>> action);

        Task<T> ExecuteWithStorageDependencyTracking<T>(string dependencyType, string dependencyName, Func<Task<T>> action);

        Task ExecuteWithStorageDependencyTracking(string dependencyType, string dependencyName, Func<Task> action);

        Task ExecuteWithDatabaseDependencyTracking(string dependencyType,
                                                   string dependencyName,
                                                   Func<Task> action);

        Task<T> ExecuteWithDatabaseDependencyTracking<T>(string dependencyType,
                                                         string dependencyName,
                                                         Func<Task<T>> action);

        Task<T> ExecuteWithRpcRequestTracking<T>(Func<Task<T>> action);

        Task ExecuteWithRpcRequestTracking(Func<Task> action, string operationName = null);

        Task<T> ExecuteWithEventHandlingTracking<T>(Func<Task<T>> action);

        Task ExecuteWithEventHandlingTracking(Func<Task> action);

        Task<T> ExecuteWithCommandHandlingTracking<T>(Func<Task<T>> action);

        Task ExecuteWithCommandHandlingTracking(Func<Task> action);

        Task<T> ExecuteWithBusinessLogicTracking<T>(string operationTargetType, string operationName, Func<Task<T>> action);

        Task ExecuteWithBusinessLogicTracking(string operationTargetType, string operationName, Func<Task> action);

        Task ExecuteWithHttpRequestTracking(Func<Task> action, Func<int> getResultStatus, Func<Exception> getException);

        Task<T> ExecuteWithHttpRequestTracking<T>(Func<Task<T>> action, Func<int> getResultStatus, Func<Exception> getException);
    }
}