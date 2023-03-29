using System.Collections.Generic;
using System.Linq;

namespace Biobanks.Submissions.Api.Services.Directory.Extensions
{
    public static class IEnumerableExtensions
    { 
    public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> items, int batchSize)
            => items
                .Select((item, index) => new { Item = item, Index = index })
                .GroupBy(x => x.Index / batchSize)
                .Select(g => g.Select(x => x.Item));

    }
}