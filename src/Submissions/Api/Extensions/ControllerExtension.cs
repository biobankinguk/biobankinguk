using Microsoft.Extensions.Caching.Memory;

namespace Biobanks.Submissions.Api.Extensions
{
    public  class ControllerExtension
    {
        private readonly IMemoryCache _memoryCache;
        
        public ControllerExtension(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
    }
    public static class TemporaryFeedbackMessageExtensions
    {
        public static void GetSiteConfigValue()
        {

        }
    }
}
