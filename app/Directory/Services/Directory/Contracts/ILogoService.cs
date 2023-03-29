using Biobanks.Entities.Data;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts
{
    public interface ILogoService
    {
        Task<Blob> GetLogoBlobAsync(string logoName);
    }
}
