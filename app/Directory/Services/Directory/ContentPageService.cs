using System;
using Biobanks.Data;
using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Biobanks.Submissions.Api.Services.Directory.Contracts;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class ContentPageService : IContentPageService
    {
        private readonly ApplicationDbContext _db;

        public ContentPageService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task Create(string title, string body, string slug, bool isEnabled)
        {
            var page = new ContentPage
            {
                Title = title,
                Body = body,
                RouteSlug = slug,
                LastUpdated = DateTime.UtcNow,
                IsEnabled = isEnabled
            };

            _db.ContentPages.Add(page);
            await _db.SaveChangesAsync();
        }

        public async Task Update(int id, string title, string body, string slug, bool isEnabled)
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
                page.IsEnabled = isEnabled;
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

        public async Task<IEnumerable<ContentPage>> ListContentPages()
            => await _db.ContentPages.ToListAsync();

        public async Task<ContentPage> GetById(int id)
            => await _db.ContentPages.FindAsync(id);

        public async Task<ContentPage> GetBySlug(string routeSlug)
            => await _db.ContentPages.FirstOrDefaultAsync(x => x.RouteSlug == routeSlug);

    }
}

