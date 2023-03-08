using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Biobanks.Submissions.Api.Filters;

/// <summary>
/// An <see cref="ActionFilterAttribute"/> that can be applied to a Biobank controller
/// to check if a biobank is suspended and show the warning message if needed.
/// </summary>
/// <remarks>
/// This attribute overrides the OnActionExecuting method of the base class and checks
/// the 'biobankId' parameter of the action's context to determine if the biobank is suspended. 
/// If it is, it sets the 'ShowSuspendedWarning' property of the controller's ViewBag to true.
/// </remarks>
public class SuspendedWarningAttribute : ActionFilterAttribute
{
  /// <summary>
  /// Overrides the <see cref="OnActionExecuting"/> method of the base class to implement the suspended warning
  /// functionality.
  /// </summary>
  /// <param name="context">The context of the action being executed.</param>
  public override async void OnActionExecuting(ActionExecutingContext context)
  {
    if (context.Controller is Controller controller)
    {
      // TODO: Make this safer - can't assume every action on the controller has a biobankId.
      int biobankId = (int)context.ActionArguments["biobankId"];
      // int.TryParse(context.ActionArguments.TryGetValue("biobankId", out var biobankId) ?? String.Empty);
      // if (!int.TryParse((string?)httpContext?.Request.RouteValues.GetValueOrDefault("biobankId") ?? string.Empty,
      //       out var biobankId))
      //   return false;
  
      var organisationService = context.HttpContext.RequestServices.GetService<IOrganisationDirectoryService>();
      
      controller.ViewBag.ShowSuspendedWarning = await organisationService.IsSuspended(biobankId);
    }
  }
}

