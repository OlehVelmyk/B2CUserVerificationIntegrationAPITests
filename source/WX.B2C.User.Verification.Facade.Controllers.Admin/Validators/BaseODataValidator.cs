using System;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.ModelBuilder;
using WX.B2C.User.Verification.Facade.Controllers.Admin.OData;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Validators
{
    public abstract class BaseODataValidator<TModel> : AbstractValidator<ODataQueryParameters> where TModel : class
    {
        private const string QueryParameterNotSupportedMsgTemplate = "The query parameter '{0}' is not supported.";
        private const string UriQueryStringInvalidMsgTemplate = "The query specified in the URI is not valid. {0}";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseODataValidator(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            
            RuleFor(query => query)
                .Custom(ValidateQueryParameters);
        }

        protected virtual void OverrideValidators(ODataQueryOptions options)
        {
            if (options.Filter is not null)
                options.Filter.Validator = new FilterQueryValidator();
        }

        protected abstract ODataValidationSettings GetValidationSettings();

        /// <remarks>
        /// OData validation exception handling from <see cref="EnableQueryAttribute.OnActionExecuting"/>
        /// </remarks>
        private void ValidateQueryParameters(ODataQueryParameters _, ValidationContext<ODataQueryParameters> context)
        {
            var request = _httpContextAccessor.HttpContext?.Request
                          ?? throw new ArgumentNullException(nameof(IHttpContextAccessor.HttpContext.Request));

            var queryValidator = new ODataQueryValidator();
            var options = GetQueryOptions(request);
            var settings = GetValidationSettings();

            OverrideValidators(options);
            OverrideContext(options, settings);

            try
            {
                queryValidator.Validate(options, settings);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                context.AddFailure(string.Format(QueryParameterNotSupportedMsgTemplate, ex.Message));
            }
            catch (NotImplementedException ex)
            {
                context.AddFailure(string.Format(UriQueryStringInvalidMsgTemplate, ex.Message));
            }
            catch (NotSupportedException ex)
            {
                context.AddFailure(string.Format(UriQueryStringInvalidMsgTemplate, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                context.AddFailure(string.Format(UriQueryStringInvalidMsgTemplate, ex.Message));
            }
        }

        private static void OverrideContext(ODataQueryOptions options, ODataValidationSettings validationSettings)
        {
            var querySettings = options.Context.DefaultQuerySettings;
            var queryOptions = validationSettings.AllowedQueryOptions;

            querySettings.MaxTop = validationSettings.MaxTop;
            querySettings.EnableCount = queryOptions.HasFlag(AllowedQueryOptions.Count);
            querySettings.EnableExpand = queryOptions.HasFlag(AllowedQueryOptions.Expand);
            querySettings.EnableFilter = queryOptions.HasFlag(AllowedQueryOptions.Filter);
            querySettings.EnableOrderBy = queryOptions.HasFlag(AllowedQueryOptions.OrderBy);
            querySettings.EnableSelect = queryOptions.HasFlag(AllowedQueryOptions.Select);
            querySettings.EnableSkipToken = queryOptions.HasFlag(AllowedQueryOptions.SkipToken);
        }

        private static ODataQueryOptions<TModel> GetQueryOptions(HttpRequest request)
        {
            var modelType = typeof(TModel);

            var builder = new ODataConventionModelBuilder(new DefaultAssemblyResolver(), true);
            var entityTypeConfiguration = builder.AddEntityType(modelType);
            builder.AddEntitySet(modelType.Name, entityTypeConfiguration);

            var edmModel = builder.GetEdmModel();
            var oDataQueryContext = new ODataQueryContext(edmModel, modelType, null);
            return new ODataQueryOptions<TModel>(oDataQueryContext, request);
        }
    }
}
