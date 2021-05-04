using Biobanks.Entities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Biobanks.Services.Contracts
{
    public interface IContentPageService
    {
        Task CreateNewContentPage(ContentPage contentPage);
        void UpdateContentPage(ContentPage contentPage);
        void DeleteContentPage(ContentPage contentPage);
        IEnumerable<ContentPage> ListContentPages(bool tracking = false,
                                    Expression<Func<ContentPage, bool>> filter = null,
                                    Func<IQueryable<ContentPage>, IOrderedQueryable<ContentPage>> orderBy = null,
                                    params Expression<Func<ContentPage, object>>[] includeProperties);
        Task<ContentPage> GetById(object id);
        Task<ContentPage> GetBySlug(object routeSlug);
    }
}
