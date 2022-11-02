using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Biobanks.Submissions.Api.Filters;

public class HandleAntiforgeryErrorAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.ExceptionHandled) return;
        
        //Handle AntiForgeryToken errors differently to the generic custom error
        if (context.Exception is AntiforgeryValidationException)
        {
            //redirect to the "You submitted the form twice!" error page
            //any routes which require a non-standard handler for HttpAntiForgeryException
            //should have it configured in their controller's OnError()
            context.Result = RedirectToRouteResult("Error");
            context.ExceptionHandled = true;
        }
        //If we've got here and it's still not handled, it should be (and HandleError's result will apply)
        context.ExceptionHandled = true;
    }
}