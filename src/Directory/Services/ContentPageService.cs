using System;
using System.Threading.Tasks;
using Biobanks.Services.Contracts;
using Biobanks.Entities.Data;
using Biobanks.Directory.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Collections.Generic;

namespace Biobanks.Services
{
    public class ContentPageService : IContentPageService
    {
        private readonly BiobanksDbContext _db;

        public ContentPageService(BiobanksDbContext db)
        {
            _db = db;
        }

        public async Task CreateNewContentPage(ContentPage contentPage)
        {            
            _db.ContentPages.Add(contentPage);
            await _db.SaveChangesAsync();
        }

        public void UpdateContentPage(ContentPage contentPage)
        {
            _db.ContentPages.Attach(contentPage);
            _db.Entry(contentPage).State = EntityState.Modified;            
        }

        public void DeleteContentPage(ContentPage contentPage)
        {
            if (_db.Entry(contentPage).State == EntityState.Detached)
            {
                _db.ContentPages.Attach(contentPage);
            }
            _db.ContentPages.Remove(contentPage);
        }

        public IEnumerable<ContentPage> ListContentPages(bool tracking = false,
                                    Expression<Func<ContentPage, bool>> filter = null,
                                    Func<IQueryable<ContentPage>, IOrderedQueryable<ContentPage>> orderBy = null,
                                    params Expression<Func<ContentPage, object>>[] includeProperties)
        {
            IQueryable<ContentPage> query = _db.ContentPages;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            if (orderBy != null)
            {
                return orderBy.Invoke(query).ToList();
            }

            return query.ToList();
        }

        public async Task<ContentPage> GetById(object id)
        {
            return await _db.ContentPages.FindAsync(id);
        }

        public async Task<ContentPage> GetBySlug(object routeSlug)
        {
            return await _db.ContentPages.FindAsync(routeSlug);
        }
    }
}
