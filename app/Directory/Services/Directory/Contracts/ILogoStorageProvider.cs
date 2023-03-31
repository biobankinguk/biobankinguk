using System.IO;
using System.Threading.Tasks;
using Biobanks.Data.Entities;

namespace Biobanks.Directory.Services.Directory.Contracts
{
    public interface ILogoStorageProvider
    {
        Task<Blob> GetLogoBlobAsync(string resourceName);

        Task<string> StoreLogoAsync(MemoryStream logo, string fileName, string contentType, string reference);

        Task RemoveLogoAsync(int organisationId);
    }
}
