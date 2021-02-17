using System;
using System.Collections.Generic;
using System.Linq;

namespace Biobanks.DataSeed.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TSource> Transform<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            return source?.Select(x =>
            {
                action(x);
                return x;
            });
        }
    }
}
