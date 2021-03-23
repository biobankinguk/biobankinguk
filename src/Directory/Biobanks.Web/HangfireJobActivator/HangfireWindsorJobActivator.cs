using System;
using System.Collections.Generic;
using Castle.MicroKernel;
using Hangfire;

namespace Biobanks.Web.HangfireJobActivator
{
    // See examples here: https://gist.github.com/IanYates/29180ce09c41e42b9a7e
    // This is modified from the second example.
    public class HangfireWindsorJobActivator : JobActivator
    {
        private readonly IKernel _kernel;

        public HangfireWindsorJobActivator(IKernel kernel)
        {
            if (kernel == null)
            {
                throw new ArgumentNullException("kernel");
            }

            _kernel = kernel;
        }

        public override object ActivateJob(Type jobType)
        {
            return _kernel.Resolve(jobType);
        }

        [Obsolete]
        public override JobActivatorScope BeginScope()
        {
            return new HangfireIocJobActivatorScope(this, _kernel);
        }

        class HangfireIocJobActivatorScope : JobActivatorScope
        {
            private readonly JobActivator _activator;
            private readonly IKernel _kernel;

            private readonly List<object> _resolvedObjects;

            public HangfireIocJobActivatorScope(JobActivator activator, IKernel iocResolver)
            {
                _activator = activator;
                _kernel = iocResolver;
                _resolvedObjects = new List<object>();
            }

            public override object Resolve(Type type)
            {
                var instance = _activator.ActivateJob(type);
                _resolvedObjects.Add(instance);
                return instance;
            }

            public override void DisposeScope()
            {
                _resolvedObjects.ForEach(_kernel.ReleaseComponent);
            }
        }
    }
}