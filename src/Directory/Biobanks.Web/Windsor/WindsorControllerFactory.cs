using Biobanks.Web.Windsor;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Biobanks.Web.Windsors
{
	/// <summary>
	/// Controller Factory class for instantiating controllers using the Windsor IoC container.
	/// </summary>
	public class WindsorControllerFactory : DefaultControllerFactory
	{
		private readonly IWindsorContainer _container;

		/// <summary>
		/// Creates a new instance of the <see cref="WindsorControllerFactory"/> class.
		/// </summary>
		/// <param name="container">The Windsor container instance to use when creating controllers.</param>
		public WindsorControllerFactory(IWindsorContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container");
			}

			AppDomain.CurrentDomain
				.GetAssemblies()
				.Where(assembly => !assembly.FullName.StartsWith("System."))
				.SelectMany(assembly => assembly.GetTypes())
				.Where(type => typeof(IController).IsAssignableFrom(type) && !type.IsInterface && type != typeof(AsyncController))
				.ToList()
				.ForEach(controllerType => container
					.Register(Component.For(controllerType)
					.Named(controllerType.FullName)
					.LifeStyle
					.Is(LifestyleType.Transient)));

			_container = container;
		}

		public override void ReleaseController(IController controller)
		{
			var disposable = controller as IDisposable;

			if (disposable != null)
			{
				disposable.Dispose();
			}

			_container.Release(controller);
		}

		protected override IController GetControllerInstance(RequestContext context, Type controllerType)
		{
			if (controllerType == null)
			{
				throw new HttpException(404, string.Format("The controller for path '{0}' could not be found or it does not implement IController.", context.HttpContext.Request.Path));
			}

			var controller = (Controller)_container.Resolve(controllerType);

			/* http://weblogs.asp.net/psteele/archive/2009/11/04/using-windsor-to-inject-dependencies-into-asp-net-mvc-actionfilters.aspx */
			if (controller != null)
			{
				controller.ActionInvoker = new WindsorActionInvoker(_container); ;
			}

			return controller;
		}
	}
}