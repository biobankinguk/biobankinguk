using System.Threading.Tasks;
using Biobanks.Data.Entities;

namespace Biobanks.Directory.Services.Directory.Contracts
{
    public interface ILogoService
    {
        Task<Blob> GetLogoBlobAsync(string logoName);
    }
}
