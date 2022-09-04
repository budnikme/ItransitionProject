using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CourseProject.Web.ExceptionFilters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception.GetType() == typeof(KeyNotFoundException))
        {
            context.ExceptionHandled = true;
            context.HttpContext.Response.StatusCode = 404;
            context.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    {"controller", "Error"},
                    {"action", "PageNotFound"},
                    {"returnUrl", context.HttpContext.Request.Path}
                });
        }
        else if (context.Exception.GetType() == typeof(UnauthorizedAccessException))
        {
            context.HttpContext.Response.StatusCode = 403;
            context.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    {"controller", "Error"},
                    {"action", "Forbidden"},
                    {"returnUrl", context.HttpContext.Request.Path}
                });
            context.ExceptionHandled = true;
        }
    }
}