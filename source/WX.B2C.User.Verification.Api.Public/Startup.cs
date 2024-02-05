using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using Autofac;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using WX.B2C.Client.Version.Validation.IoC;
using WX.B2C.User.Verification.Api.Public.Extensions;
using WX.B2C.User.Verification.Api.Public.Helpers;
using WX.B2C.User.Verification.Api.Public.IoC;
using WX.B2C.User.Verification.Facade.Controllers.Public.Filters;
using WX.B2C.User.Verification.Facade.Controllers.Public.Validators;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Authentication;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Filters;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Metrics;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.OperationContext;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Routing;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;

namespace WX.B2C.User.Verification.Api.Public
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
                        options.JsonSerializerOptions.PropertyNamingPolicy = SnakeCaseJsonNamingPolicy.Instance;
                        options.JsonSerializerOptions.DictionaryKeyPolicy = SnakeCaseJsonNamingPolicy.Instance;
                        options.JsonSerializerOptions.IgnoreNullValues = true;
                    })
                    .AddFluentValidation();

            services.RegisterClientDeprecationMiddleware(Configuration);
            services.RegisterResourceGoneMiddleware();

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

            services.AddAuthentication(options =>
                    {
                        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultForbidScheme = "ForbidScheme";
                        options.AddScheme<DefaultForbidAuthenticationHandler>(options.DefaultForbidScheme, "Forbidden");
                    })
                    .AddJwtBearer(options =>
                    {
                        options.Events = new JwtBearerEvents { OnTokenValidated = JwtBearerEventsHandler.OnTokenValidated };
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateLifetime = true,
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateIssuerSigningKey = false,
                            SignatureValidator = (token, parameters) => new JwtSecurityToken(token)
                        };
                    });

            services.AddSwagger(ServiceDefinitions.ProjectName);
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
            builder.RegisterModule<PublicApiModule>();
            builder.RegisterOperationContextScope();
            builder.RegisterMetricsReporting();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.RoutePrefix = "metadata";

                    // Build a swagger endpoint for each discovered API version
                    var apiVersionDescription = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
                    var versionDescriptions = apiVersionDescription.ApiVersionDescriptions;
                    var endpoints = versionDescriptions.BuildSwaggerEndpoints(ServiceDefinitions.ProjectName);
                    options.SwaggerEndpoints(endpoints);
                });
            }

            app.UseClientDeprecationMiddleware();
            app.UseOperationContextScope();
            app.UseMetricsReporting();
            app.UseExceptionHandler("/error");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResourceGoneMiddleware();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
