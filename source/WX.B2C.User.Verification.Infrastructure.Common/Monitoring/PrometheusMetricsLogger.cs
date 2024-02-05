using System;
using System.Net;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Infrastructure.Common.Configuration;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Infrastructure.Common.Monitoring
{
    internal partial class PrometheusMetricsLogger : IMetricsLogger
    {
        private const string BusinessLogicType = "BusinessLogic";
        private const string SelfType = "Self";
        private const string ServiceType = "Service";
        private const string CacheType = "Cache";
        private const string StorageType = "Storage";
        private const string DatabaseType = "Database";

        private readonly IPrometheusMetricsDashboard _dashboard;
        private readonly IOperationContextProvider _operationContextProvider;
        
        private readonly string _applicationHost;

        public PrometheusMetricsLogger(IPrometheusMetricsDashboard dashboards,
                                       IHostSettingsProvider hostSettingsProvider,
                                       IOperationContextProvider operationContextProvider)
        {
            
            if (hostSettingsProvider == null)
                throw new ArgumentNullException(nameof(hostSettingsProvider));

            _dashboard = dashboards ?? throw new ArgumentNullException(nameof(dashboards));
            _operationContextProvider = operationContextProvider ?? throw new ArgumentNullException(nameof(operationContextProvider));
            _applicationHost = hostSettingsProvider.GetSetting(HostSettingsKey.ApplicationHost);
        }

        private string OperationName => _operationContextProvider.GetContextOrDefault().OperationName;

        public Task<T> ExecuteWithServiceDependencyTracking<T>(string operationProtocol,
                                                               string operationTargetType,
                                                               string operationName,
                                                               Func<Task<T>> action,
                                                               bool throttle = false) =>
            ExecuteWithOperationTracking(ServiceType, operationProtocol, operationTargetType, operationName, action);

        public Task ExecuteWithServiceDependencyTracking(string operationProtocol,
                                                         string operationTargetType,
                                                         string operationName,
                                                         Func<Task> action,
                                                         bool throttle = false) =>
            ExecuteWithOperationTracking(ServiceType, operationProtocol, operationTargetType, operationName, action);

        public Task ExecuteWithCacheDependencyTracking(string dependencyType, string dependencyName, Func<Task> action) =>
            ExecuteWithOperationTracking(CacheType, OperationProtocols.HTTP, dependencyType, dependencyName, action);

        public Task<T> ExecuteWithCacheDependencyTracking<T>(string dependencyType, string dependencyName, Func<Task<T>> action) =>
            ExecuteWithOperationTracking(CacheType, OperationProtocols.HTTP, dependencyType, dependencyName, action);

        public Task<T> ExecuteWithStorageDependencyTracking<T>(string dependencyType, string dependencyName, Func<Task<T>> action) =>
            ExecuteWithOperationTracking(StorageType, OperationProtocols.HTTP, dependencyType, dependencyName, action);

        public Task ExecuteWithStorageDependencyTracking(string dependencyType, string dependencyName, Func<Task> action) =>
            ExecuteWithOperationTracking(StorageType, OperationProtocols.HTTP, dependencyType, dependencyName, action);

        public Task ExecuteWithDatabaseDependencyTracking(string dependencyType,
                                                          string dependencyName,
                                                          Func<Task> action) =>
            ExecuteWithOperationTracking(DatabaseType, OperationProtocols.HTTP, dependencyType, dependencyName, action);

        public Task<T> ExecuteWithDatabaseDependencyTracking<T>(string dependencyType,
                                                                string dependencyName,
                                                                Func<Task<T>> action) =>
            ExecuteWithOperationTracking(DatabaseType, OperationProtocols.HTTP, dependencyType, dependencyName, action);

        public Task<T> ExecuteWithRpcRequestTracking<T>(Func<Task<T>> action) =>
            ExecuteWithOperationTracking(SelfType,
                                         OperationProtocols.RPC,
                                         _applicationHost,
                                         OperationName,
                                         action);

        public Task ExecuteWithRpcRequestTracking(Func<Task> action, string operationName = null) =>
            ExecuteWithOperationTracking(SelfType,
                                         OperationProtocols.RPC,
                                         _applicationHost,
                                         operationName ?? OperationName,
                                         action);

        public Task<T> ExecuteWithEventHandlingTracking<T>(Func<Task<T>> action) =>
            ExecuteWithOperationTracking(SelfType,
                                         OperationProtocols.Event,
                                         _applicationHost,
                                         OperationName,
                                         action);

        public Task ExecuteWithEventHandlingTracking(Func<Task> action) =>
            ExecuteWithOperationTracking(SelfType,
                                         OperationProtocols.Event,
                                         _applicationHost,
                                         OperationName,
                                         action);

        public Task<T> ExecuteWithCommandHandlingTracking<T>(Func<Task<T>> action) =>
            ExecuteWithOperationTracking(SelfType,
                                         OperationProtocols.Command,
                                         _applicationHost,
                                         OperationName,
                                         action);

        public Task ExecuteWithCommandHandlingTracking(Func<Task> action) =>
            ExecuteWithOperationTracking(SelfType,
                                         OperationProtocols.Command,
                                         _applicationHost,
                                         OperationName,
                                         action);

        public Task<T> ExecuteWithBusinessLogicTracking<T>(string operationTargetType, string operationName, Func<Task<T>> action) =>
            ExecuteWithOperationTracking(BusinessLogicType, OperationProtocols.InMemory, operationTargetType, operationName, action);

        public Task ExecuteWithBusinessLogicTracking(string operationTargetType, string operationName, Func<Task> action) =>
            ExecuteWithOperationTracking(BusinessLogicType, OperationProtocols.InMemory, operationTargetType, operationName, action);

        public Task ExecuteWithHttpRequestTracking(Func<Task> action, Func<int> getResultStatus, Func<Exception> getException)
        {
            var track = new DisposableTrack(this,
                                            SelfType,
                                            OperationProtocols.HTTP,
                                            _applicationHost,
                                            OperationName,
                                            true);
            return track.Wrap(action,
                              exception => { TrackHttpCallException(getResultStatus, track, exception); },
                              () => { TrackHttpSuccessCall(getResultStatus, getException, track); });
        }
        
        public Task<T> ExecuteWithHttpRequestTracking<T>(Func<Task<T>> action, 
                                                         Func<int> getResultStatus, 
                                                         Func<Exception> getException)
        {
            var track = new DisposableTrack(this,
                                            SelfType,
                                            OperationProtocols.HTTP,
                                            _applicationHost,
                                            OperationName,
                                            true);
            return track.Wrap(action,
                              exception => { TrackHttpCallException(getResultStatus, track, exception); },
                              () => { TrackHttpSuccessCall(getResultStatus, getException, track); });
        }

        private Task ExecuteWithOperationTracking(string target,
                                                  string protocol,
                                                  string type,
                                                  string name,
                                                  Func<Task> action)
        {
            var track = new DisposableTrack(this, target, protocol, $"{type}", $"{name}", false);
            return track.Wrap(action,
                              exception =>
                              {
                                  track.Exception = exception;
                                  track.StatusCode = HttpStatusCode.InternalServerError;
                              });
        }

        private Task<T> ExecuteWithOperationTracking<T>(string target,
                                                        string protocol,
                                                        string dependencyType,
                                                        string dependencyName,
                                                        Func<Task<T>> action)
        {
            var track = new DisposableTrack(this, target, protocol, $"{dependencyType}", $"{dependencyName}", false);

            return track.Wrap(action,
                              exception =>
                              {
                                  track.Exception = exception;
                                  track.StatusCode = HttpStatusCode.InternalServerError;
                              });
        }

        private static void TrackHttpCallException(Func<int> getResultStatus, DisposableTrack track, Exception exception)
        {
            var code = getResultStatus();
            track.Exception = exception;
            track.StatusCode = MapStatusCode(code);
        }

        private static void TrackHttpSuccessCall(Func<int> getResultStatus, Func<Exception> getException, DisposableTrack track)
        {
            var code = getResultStatus();
            var statusCode = MapStatusCode(code);
            //https://docs.microsoft.com/en-us/uwp/api/windows.web.http.httpresponsemessage.issuccessstatuscode?view=winrt-22000#property-value
            var isSuccessResponse = code is >= 200 and <= 299;
            if (!isSuccessResponse)
                track.Exception = getException();
            track.StatusCode = statusCode;
        }

        private static HttpStatusCode MapStatusCode(int code)
        {
            return code == 0 ? HttpStatusCode.InternalServerError : (HttpStatusCode) code;
        }
    }
}