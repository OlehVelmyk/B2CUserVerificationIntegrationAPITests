using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Responses
{
    public class PageResult<T>
    {
        public T[] Items { get; set; }

        [NotRequired]
        public string NextPageLink { get; set; }

        public int TotalCount { get; set; }
    }
}
