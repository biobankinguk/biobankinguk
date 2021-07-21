using Biobanks.Entities.Api;

namespace Biobanks.Extensions
{
    public static class SampleExtensions
    {
        //TODO: Try find a solution where this can be used in a EF LINQ WHERE statement
        public static bool IsExtracted(this Sample sample)
            => !string.IsNullOrEmpty(sample.SampleContentId);
    }
}
