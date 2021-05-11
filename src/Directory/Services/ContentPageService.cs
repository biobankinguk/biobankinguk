using System;
using System.Threading.Tasks;
using Biobanks.Services.Contracts;
using Biobanks.Entities.Data;
using Biobanks.Directory.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Collections.Generic;
using System.Text;
using Biobanks.Directory.Data.Repositories;


namespace Biobanks.Services
{
    public class ContentPageService : IContentPageService
    {
        private readonly BiobanksDbContext _db;

        public ContentPageService(BiobanksDbContext db)
        {
            _db = db;
        }

        public async Task Create(string title, string body, string slug)
        {
            var page = new ContentPage
            {
                Title = title,
                Body = body,
                RouteSlug = slug,
                LastUpdated = DateTime.UtcNow
            };            

            _db.ContentPages.Add(page);
            await _db.SaveChangesAsync();
        }

        public async Task Update(int id, string title, string body, string slug)
        {
            var page = await _db.ContentPages.FindAsync(id);
            if (page == null)
            {
                throw new KeyNotFoundException();
            }
            else
            {
                page.Title = title;
                page.Body = body;
                page.RouteSlug = slug;
                page.LastUpdated = DateTime.UtcNow;
                await _db.SaveChangesAsync();                
            }                        
        }

        public async Task Delete(int id)
        {
            var page = await _db.ContentPages.FindAsync(id);
            if (page != null)
            {
                _db.ContentPages.Remove(page);
                await _db.SaveChangesAsync();
            }                                   
        }

        public IEnumerable<ContentPage> ListContentPages()
        {
            IQueryable<ContentPage> query = _db.ContentPages;
            return query.ToList();
        }
     
        public async Task<ContentPage> GetById(int id)
            => await _db.ContentPages.FindAsync(id);
          
        

        public async Task<ContentPage> GetBySlug(string routeSlug)
            => await _db.ContentPages.FirstOrDefaultAsync(x => x.RouteSlug == routeSlug);
        

    }
}
