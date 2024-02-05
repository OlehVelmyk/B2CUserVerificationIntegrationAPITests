using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Autofac.Core;
using Autofac.Core.Lifetime;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;

namespace WX.B2C.User.Verification.Infrastructure.Common
{
    internal static class DependencyInjectionIssueDetector
    {
        private const string ApplicationNamespacePrefix = "WX.B2C.User.Verification";

        public static void CheckAfterBuild(ContainerBuilder builder)
        {
            builder.RegisterBuildCallback(CheckAllInjections);
        }

        private static void CheckAllInjections(IContainer container)
        {
            if (!ShouldCheck(container))
                return;

            var resolvingIssues = new Dictionary<string, List<string>>();
            foreach (var registration in container.ComponentRegistry.Registrations)
            {
                //Skip types which cannot be resolved in this scope. Usually it is a service fabric services
                if (registration.Lifetime is MatchingScopeLifetime)
                    continue;

                foreach (var service in registration.Services.OfType<IServiceWithType>())
                {
                    var fullName = service.ServiceType.FullName;
                    var ownService = fullName.StartsWith(ApplicationNamespacePrefix);
                    var isEventHandler = fullName == "WX.Messaging.Subscriber.EventHub.Interfaces.IGenericEventProcessorFactory";
                    if (!ownService && !isEventHandler)
                        continue;

                    try
                    {
                        if (service is KeyedService keyedService)
                        {
                            _ = container.ResolveKeyed(keyedService.ServiceKey, service.ServiceType);
                            continue;
                        }
                        _ = container.Resolve(service.ServiceType);
                    }
                    catch (DependencyResolutionException e)
                    {
                        if (IsKnownIssue(e))
                            continue;

                        var sb = new StringBuilder($"{fullName}: {e.Message}");
                        var rootProblem = BuildErrorMessage(e, sb);
                        if (!resolvingIssues.ContainsKey(rootProblem))
                            resolvingIssues[rootProblem] = new List<string>();

                        resolvingIssues[rootProblem].Add(sb.ToString());
                    }
                }
            }

            ReportAboutIssues(resolvingIssues);
        }

        private static void ReportAboutIssues(Dictionary<string, List<string>> resolvingIssues)
        {
            if (resolvingIssues.Count <= 0)
                return;

            var sb = new StringBuilder();
            var host = System.Reflection.Assembly.GetEntryAssembly().FullName;
            sb.AppendLine($"IoC issues in {host}");

            foreach (var resolvingIssue in resolvingIssues)
            {
                sb.AppendLine(resolvingIssue.Key);

                if (resolvingIssue.Key.Length > 1)
                    sb.AppendJoin($"{Environment.NewLine}\t", resolvingIssue.Value);

                sb.AppendLine("-----------------------");
            }

            var message = sb.ToString();
            throw new ApplicationException(message);
        }

        /// <summary>
        /// Need to filter some specific exceptions which always happen.
        /// </summary>
        private static bool IsKnownIssue(DependencyResolutionException exception)
        {
            if (exception.Message.Contains("OperationContextMiddleware"))
                return true;
            return false;
        }

        private static bool ShouldCheck(IContainer container)
        {
            var config = container.Resolve<IAppConfig>();
            return config.CheckAutofacRegistrations;
        }

        /// <summary>
        /// Concat messages including inner exceptions and return the deepest message.
        /// </summary>
        private static string BuildErrorMessage(Exception exception, StringBuilder stringBuilder)
        {
            if (exception.InnerException != null)
            {
                stringBuilder.AppendLine($"→{exception.InnerException.Message}");
                return BuildErrorMessage(exception.InnerException, stringBuilder);
            }

            return exception.Message;
        }
    }
}
