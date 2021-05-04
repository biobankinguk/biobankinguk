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
                                    Expression<Func<ContentPage, bool>> filter = null);
        Task<ContentPage> GetById(int id);
        Task<ContentPage> GetBySlug(string routeSlug);
    }
}
