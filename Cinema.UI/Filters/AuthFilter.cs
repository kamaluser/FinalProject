﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cinema.UI.Filters
{
    public class AuthFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller as ControllerBase;
            if (context.HttpContext.Request.Cookies["token"] == null)
            {
                context.Result = controller.RedirectToAction(
             actionName: "Login",
             controllerName: "Account",
             new { returnUrl = context.HttpContext.Request.Path }
             );
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do something after the action executes.
        }
    }
}
