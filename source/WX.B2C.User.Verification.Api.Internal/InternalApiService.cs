﻿using System.Collections.Generic;
using System.Fabric;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace WX.B2C.User.Verification.Api.Internal
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    public class InternalApiService : StatelessService
    {
        public InternalApiService(StatelessServiceContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        return new WebHostBuilder()
                               .UseKestrel()
                               .ConfigureServices(services =>
                               {
                                   services.AddSingleton(serviceContext);
                                   services.AddAutofac();
                               })
                               .UseContentRoot(Directory.GetCurrentDirectory())
                               .UseStartup<Startup>()
                               .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                               .UseUrls(url)
                               .Build();
                    }))
            };
        }
    }
}