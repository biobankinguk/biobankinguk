using Castle.Windsor;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Async;

namespace Biobanks.Web.Windsor
{
    public class WindsorActionInvoker : AsyncControllerActionInvoker
    {
        readonly IWindsorContainer _container;

        public WindsorActionInvoker(IWindsorContainer container)
        {
            _container = container;
        }

        protected override System.IAsyncResult BeginInvokeActionMethodWithFilters(ControllerContext controllerContext, IList<IActionFilter> filters, ActionDescriptor actionDescriptor, IDictionary<string, object> parameters, System.AsyncCallback callback, object state)
        {
            foreach (IActionFilter actionFilter in filters)
            {
                _container.Kernel.InjectProperties(actionFilter);
            }

            return base.BeginInvokeActionMethodWithFilters(controllerContext, filters, actionDescriptor, parameters, callback, state);
        }

        protected override ActionExecutedContext InvokeActionMethodWithFilters(ControllerContext controllerContext, IList<IActionFilter> filters, ActionDescriptor actionDescriptor, IDictionary<string, object> parameters)
        {
            foreach (IActionFilter actionFilter in filters)
            {
                _container.Kernel.InjectProperties(actionFilter);
            }

            return base.InvokeActionMethodWithFilters(controllerContext, filters, actionDescriptor, parameters);
        }

        protected override AuthorizationContext InvokeAuthorizationFilters(ControllerContext controllerContext, IList<IAuthorizationFilter> filters, ActionDescriptor actionDescriptor)
        {
            foreach (IAuthorizationFilter authorizeFilter in filters)
            {
                _container.Kernel.InjectProperties(authorizeFilter);
            }

            return base.InvokeAuthorizationFilters(controllerContext, filters, actionDescriptor);
        }
    }
}