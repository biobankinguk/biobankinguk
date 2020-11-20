using StackExchange.Profiling;
using StackExchange.Profiling.EntityFramework6;
using StackExchange.Profiling.Mvc;
using System.Configuration;

namespace Biobanks.Web.App_Start
{
    public static class ProfilerConfig
    {
        public static bool ProfilerEnabled => ConfigurationManager.AppSettings["EnvironmentName"] == "Development";

        public static void Configure()
        {    
            if (ProfilerEnabled)
            {
                MiniProfiler.Configure(new MiniProfilerOptions
                    {
                        ResultsAuthorize = request => request.IsAuthenticated
                    }
                    .AddViewProfiling()
                ); ;

                MiniProfilerEF6.Initialize();
            }
        }

        public static void Start() 
        {
            if (ProfilerEnabled)
            {
                MiniProfiler.StartNew();
            }
        }

        public static void Stop() 
        {
            MiniProfiler.Current?.Stop();
        }
    }
}