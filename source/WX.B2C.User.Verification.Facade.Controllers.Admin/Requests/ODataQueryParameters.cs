using Microsoft.AspNetCore.Mvc;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Requests
{
    public class ODataQueryParameters
    {
        [NotRequired]
        [BindProperty(Name = "$filter")]
        public string Filter { get; set; }

        [NotRequired]
        [BindProperty(Name = "$orderby")]
        public string OrderBy { get; set; }

        [NotRequired]
        [BindProperty(Name = "$top")]
        public int? Top { get; set; }

        [NotRequired]
        [BindProperty(Name = "$skip")]
        public int? Skip { get; set; }

        [NotRequired]
        [BindProperty(Name = "$select")]
        public string Select { get; set; }

        [NotRequired]
        [BindProperty(Name = "$expand")]
        public string Expand { get; set; }
    }
}
