using System;
using Core.Submissions.Models;
using Core.Submissions.Types;

namespace Biobanks.Submissions.Api
{
    /// <summary>
    /// General Utility methods
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Calculate Pagination properties of a given model which supports them
        /// </summary>
        /// <typeparam name="T">Type of the Paginated Model</typeparam>
        /// <param name="paginatedResourceUri"></param>
        /// <param name="paging"></param>
        /// <param name="count"></param>
        /// <param name="total"></param>
        /// <param name="model"></param>
        public static void Paginate<T>(
            Uri paginatedResourceUri,
            PaginationParams paging,
            int count, int total, ref T model)
            where T : BasePaginatedModel
        {
            //prep next/previous
            var nextOffset = paging.Offset + count;

            var previousLimit = paging.Limit;
            var previousOffset = paging.Offset - previousLimit;
            if (previousOffset < 0)
            {
                previousLimit = paging.Offset;
                previousOffset = 0;
            }

            //update the model
            model.Count = count;
            model.Offset = paging.Offset;
            model.Total = total;
            model.Next = nextOffset < total
                ? GetPaginatedUri(
                    paginatedResourceUri,
                    new PaginationParams
                    {
                        Offset = nextOffset,
                        Limit = paging.Limit
                    })
                : null;
            model.Previous = paging.Offset > 0
                ? GetPaginatedUri(
                    paginatedResourceUri,
                    new PaginationParams
                    {
                        Offset = previousOffset,
                        Limit = previousLimit
                    })
                : null;
        }

        private static Uri GetPaginatedUri(
            Uri paginatedResourceUri, PaginationParams paging)
            => new Uri(new UriBuilder(paginatedResourceUri)
                    {
                        Query = $"offset={paging.Offset}&limit={paging.Limit}"
                    }
                    .Uri
                    .PathAndQuery,
                UriKind.Relative);
    }
}
