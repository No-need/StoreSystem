using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;

namespace StoreSystem.Filter
{
    public class AuthFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //該路徑是否不需任何檢核
            var isAllowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (isAllowAnonymous) return;
            var userId = context.HttpContext.Session.GetString("UserId");
            if(string.IsNullOrEmpty(userId) ) {
                var urlHelperFactory = context.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
                var urlHelper = urlHelperFactory.GetUrlHelper(context);
                context.Result = new RedirectResult(urlHelper.Action("login", "Store"));
            }
        }
    }
}
