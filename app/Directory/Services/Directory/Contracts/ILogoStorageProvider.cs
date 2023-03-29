using Biobanks.Entities.Data;
using System.IO;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts
{
    public interface ILogoStorageProvider
    {
        Task<Blob> GetLogoBlobAsync(string resourceName);

        Task<string> StoreLogoAsync(MemoryStream logo, string fileName, string contentType, string reference);

        Task RemoveLogoAsync(int organisationId);
    }
}
