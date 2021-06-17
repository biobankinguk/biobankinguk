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
        Task Create(string title, string body, string slug, bool isEnabled);
        Task Update(int id, string title, string body, string slug, bool isEnabled);
        Task Delete(int id);
        Task<IEnumerable<ContentPage>> ListContentPages();
        Task<ContentPage> GetById(int id);
        Task<ContentPage> GetBySlug(string routeSlug);
    }
}
