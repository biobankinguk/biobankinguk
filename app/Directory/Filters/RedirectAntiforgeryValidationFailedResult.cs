using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Biobanks.Directory.Filters;

public class RedirectAntiforgeryValidationFailedResult : IAlwaysRunResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        // Handle AntiForgeryToken errors differently to the generic custom error
        if (context.Result is IAntiforgeryValidationFailedResult result)
        {
            // redirect to the "You submitted the form twice!" error page
            context.Result = new RedirectToRouteResult(new RouteValueDictionary
            {
                {"action", "CSRFToken"},
                {"controller", "Error"},
            });
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    { }
}
