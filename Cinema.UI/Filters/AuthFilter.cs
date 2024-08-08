using Cinema.UI.Extensions;
using Cinema.UI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cinema.UI.Filters
{
    public class AuthFilter : IActionFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICrudService _crudService;

        public AuthFilter(IHttpContextAccessor httpContextAccessor, ICrudService crudService)
        {
            _httpContextAccessor = httpContextAccessor;
            _crudService = crudService;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var token = _httpContextAccessor.HttpContext.Request.Cookies["token"];
            if (token == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl = context.HttpContext.Request.Path });
                return;
            }

            var needsPasswordReset = _httpContextAccessor.HttpContext.Session.GetBool("NeedsPasswordReset");
            if (needsPasswordReset)
            {
                context.Result = new RedirectToActionResult("ResetPassword", "Account", new { returnUrl = context.HttpContext.Request.Path });
                return;
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do something after the action executes.
        }
    }
}
