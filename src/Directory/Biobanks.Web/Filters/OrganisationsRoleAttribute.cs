using System;
using System.Web.Mvc;
using Biobanks.Services.Contracts;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using StackExchange.Profiling;

namespace Biobanks.Web.Filters
{
    public class OrganisationsRoleAttribute : FilterAttribute, IActionFilter, IResultFilter
    {
        public IBiobankReadService BiobankReadService { get; set; }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // Filter Conditions
            if (!filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                using (MiniProfiler.Current.Step("Filter: OrganisationsRoleAttribute"))
                {
                    var userId = filterContext.RequestContext.HttpContext.User.Identity.GetUserId();

                    var biobankRequests = BiobankReadService.GetAcceptedBiobankRequestIdsAndNamesByUserId(userId);
                    var networkRequests = BiobankReadService.GetAcceptedNetworkRequestIdsAndNamesByUserId(userId);

                    var biobanks = BiobankReadService.GetBiobankIdsAndNamesByUserId(userId);
                    var networks = BiobankReadService.GetNetworkIdsAndNamesByUserId(userId);

                    filterContext.Controller.ViewBag.UserBiobankRequests = JsonConvert.SerializeObject(biobankRequests);
                    filterContext.Controller.ViewBag.UserNetworkRequests = JsonConvert.SerializeObject(networkRequests);

                    filterContext.Controller.ViewBag.UserBiobanks = JsonConvert.SerializeObject(biobanks);
                    filterContext.Controller.ViewBag.UserNetworks = JsonConvert.SerializeObject(networks);
                }
            }
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}
