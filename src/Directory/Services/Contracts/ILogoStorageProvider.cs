using System.IO;
using System.Threading.Tasks;
using Directory.Entity.Data;

namespace Directory.Services.Contracts
{
    public interface ILogoStorageProvider
    {
        Task<Blob> GetLogoBlobAsync(string resourceName);

        Task<string> StoreLogoAsync(MemoryStream logo, string fileName, string contentType, string reference);

        Task RemoveLogoAsync(int organisationId);
    }
}
