using System.Text.Json.Serialization;
using Autofac;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WX.B2C.User.Verification.Api.Internal.Extensions;
using WX.B2C.User.Verification.Api.Internal.IoC;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Filters;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Filters;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Metrics;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.OperationContext;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Routing;

namespace WX.B2C.User.Verification.Api.Internal
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
                        options.JsonSerializerOptions.IgnoreNullValues = true;
                    })
                    .AddFluentValidation(fluentConfig =>
                    {
                        fluentConfig.RegisterValidatorsFromAssembly(typeof(ValidationFilter).Assembly);
                    });

            services.AddApiVersioning(options =>
                    {
                        options.ReportApiVersions = true;
                        options.AssumeDefaultVersionWhenUnspecified = true;
                        options.DefaultApiVersion = new ApiVersion(1, 0);
                    })
                    .AddVersionedApiExplorer(options =>
                    {
                        options.GroupNameFormat = "'v'VVV";
                        options.SubstituteApiVersionInUrl = true;
                    });

            services.AddSwagger(ServiceDefinitions.ProjectName);
            services.AddSwaggerGen(options =>
            {
                options.UseAllOfForInheritance();
            });
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
            builder.RegisterModule<InternalApiModule>();
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

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}