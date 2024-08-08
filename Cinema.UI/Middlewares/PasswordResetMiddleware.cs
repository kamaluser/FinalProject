using Cinema.UI.Extensions;

namespace Cinema.UI.Middlewares
{
    public class PasswordResetMiddleware
    {
        private readonly RequestDelegate _next;

        public PasswordResetMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/admin") &&
                !context.Session.GetBool("NeedsPasswordReset") &&
                context.Request.Cookies.ContainsKey("token"))
            {
                context.Response.Redirect("/Account/ResetPassword");
                return;
            }

            await _next(context);
        }
    }

}
