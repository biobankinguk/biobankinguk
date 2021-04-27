using System;
using System.Threading.Tasks;
using Biobanks.Services.Contracts;
using Biobanks.Entities.Data;
using Biobanks.Directory.Data;

namespace Biobanks.Services
{
    public class ContentPageService : IContentPageService
    {
        private readonly BiobanksDbContext _db;

        public ContentPageService(BiobanksDbContext db)
        {
            _db = db;
        }

        public async Task CreateNewContentPage(string routeSlug, string title, string body)
        {
            var contentPage = new ContentPage
            {
                RouteSlug = routeSlug,
                Title = title,
                Body = body,
                LastUpdated = DateTime.Now,
                IsEnabled = true
            };
            _db.ContentPages.Add(contentPage);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateContentPage(string routeSlug, string title, string body)
        {
            var contentPage = new ContentPage
            {
                RouteSlug = routeSlug,
                Title = title,
                Body = body,
                LastUpdated = DateTime.Now,
                IsEnabled = true
            };    
        }
    }
}
