using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biobanks.Directory.Data.Repositories;
using Biobanks.Services.Contracts;
using Biobanks.Entities.Data;


namespace Biobanks.Services
{
    public class ContentPageService : IContentPageService
    {
        private readonly IGenericEFRepository<ContentPage> _contentPageRepository;

        public ContentPageService(IGenericEFRepository<ContentPage> contentPageRepository)
        {
            _contentPageRepository = contentPageRepository;
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
            _contentPageRepository.Insert(contentPage);

            await _contentPageRepository.SaveChangesAsync();
        }
    }
}
