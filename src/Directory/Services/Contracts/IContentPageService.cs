using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biobanks.Services.Contracts
{
    public interface IContentPageService
    {
        Task CreateNewContentPage(string routeSlug, string title, string body);
        Task UpdateContentPage(string routeSlug, string title, string body);
    }
}
