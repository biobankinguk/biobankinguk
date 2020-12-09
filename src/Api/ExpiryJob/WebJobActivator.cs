using System;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;

namespace Biobanks.ExpiryJob
{
    public class WebJobsActivator : IJobActivator
    {
        private readonly IServiceProvider _services;
 
        public WebJobsActivator(IServiceProvider services)
        {
            _services = services;
        }

        public T CreateInstance<T>() => _services.GetService<T>();
    }
}
