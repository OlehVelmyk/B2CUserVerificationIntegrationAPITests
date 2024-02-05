using System.Fabric;
using System.Globalization;
using System.Text.Json.Serialization;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WX.Admin.Authentication;
using WX.Admin.Authentication.AccessGroupAuthorization;
using WX.B2C.User.Verification.Api.Admin.Extensions;
using WX.B2C.User.Verification.Api.Admin.IoC;
using WX.B2C.User.Verification.Api.Admin.Options;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Converters;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Filters;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Authentication;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Converters;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Filters;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Metrics;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.OperationContext;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Routing;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Extensions;
using WX.B2C.User.Verification.Infrastructure.Common.Configuration;

namespace WX.B2C.User.Verification.Api.Admin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; })
                    .AddControllers(options =>
                    {
                        options.Filters.Add(new ValidationFilter());
                        options.Filters.Add<RequestLoggingFilter>();
                        options.Filters.Add(new ProducesAttribute("application/json"));
                        options.Conventions.Add(new RouteTokenTransformerConvention(new ToKebabTransformer()));
                    })
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                        options.JsonSerializerOptions.Converters.Add(new CollectionStepRequestConverter());
                        options.JsonSerializerOptions.Converters.Add(new JobScheduleConverter());
                        options.JsonSerializerOptions.Converters.Add(new PolymorphicSerializer<CollectionStepVariantDto>());
                        options.JsonSerializerOptions.IgnoreNullValues = true;
                    })
                    .AddFluentValidation();

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            }).AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddSwagger(ServiceDefinitions.ProjectName);
            services.AddSwaggerGen(options =>
            {
                options.UseAllOfForInheritance();

                options.UsePolymorphismFor<CollectionStepVariantDto>(nameof(CollectionStepVariantDto.Type));
            });

            services.AddAuthentication(options =>
            {
                options.DefaultForbidScheme = "ForbidScheme";
                options.AddScheme<DefaultForbidAuthenticationHandler>(options.DefaultForbidScheme, "Forbidden");
            });

            var fabricHostSettingsProvider = new FabricHostSettingsProvider(FabricRuntime.GetActivationContext());
            var aadInstance = fabricHostSettingsProvider.GetSetting("ida:AADInstance");
            var clientId = fabricHostSettingsProvider.GetSetting("ida:ClientId");
            var tenantId = fabricHostSettingsProvider.GetSetting("ida:TenantId");
            var audience = fabricHostSettingsProvider.GetSetting("ida:Audience");
            services.AddAccessGroupAuthentication(_ => new FilterConfiguration
            {
                Audience = audience,
                Authority = string.Format(CultureInfo.InvariantCulture, aadInstance, tenantId),
                Tenant = tenantId,
                AadInstance = aadInstance,
                ClientId = clientId
            });
            
            services.ConfigureHttpRedirects();
            services.ConfigureKestrelHttps();
        }

        /// <summary>
        /// ConfigureContainer is where you can register things directly
        /// with Autofac. This runs after ConfigureServices so the things
        /// here will override registrations made in ConfigureServices.
        /// Don't build the container; that gets done for you by the factory.
        /// </summary>
        /// <param name="builder"></param>
        /// <remarks.>https://autofaccn.readthedocs.io/en/latest/integration/aspnetcore.html#startup-class</remarks.>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterOperationContextScope();
            builder.RegisterMetricsReporting();
            builder.RegisterModule<AdminApiModule>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.RoutePrefix = string.Empty;

                    // Build a swagger endpoint for each discovered API version
                    var apiVersionDescription = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
                    var versionDescriptions = apiVersionDescription.ApiVersionDescriptions;
                    var endpoints = versionDescriptions.BuildSwaggerEndpoints(ServiceDefinitions.ProjectName);
                    options.SwaggerEndpoints(endpoints);
                });
            }

            app.UseOperationContextScope();
            app.UseMetricsReporting();
            app.UseExceptionHandler("/error");
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
