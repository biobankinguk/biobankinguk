using System;
using Biobanks.Submissions.Core.Models;
using Biobanks.Submissions.Api.Types;

namespace Biobanks.Submissions.Api
{
    public static class Utils
    {
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
