using Biobanks.Entities.Api;

namespace Biobanks.Extensions
{
    public static class SampleExtensions
    {
        public static bool IsExtracted(this Sample sample)
            => !string.IsNullOrEmpty(sample.SampleContentId);
    }
}
