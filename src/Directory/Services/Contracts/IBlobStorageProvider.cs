using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Directory.Entity.Data;

namespace Directory.Services.Contracts
{
    public interface IBlobStorageProvider
    {
        Task<Blob> GetBlobAsync(string resourceName);

        Task<IEnumerable<Blob>> GetBlobsAsync(string resourceWildcard);

        Task<string> StoreBlobAsync(MemoryStream blob, string fileName, string contentType, string reference);

        Task DeleteBlobAsync(string resourceName);
    }
}
