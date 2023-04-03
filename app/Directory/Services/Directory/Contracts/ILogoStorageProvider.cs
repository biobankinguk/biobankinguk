using System.IO;
using System.Threading.Tasks;
using Biobanks.Data.Entities;

namespace Biobanks.Directory.Services.Directory.Contracts
{
    public interface ILogoStorageProvider
    {
        Task<Blob> GetLogoBlob(string resourceName);

        Task<string> StoreLogo(MemoryStream logo, string fileName, string contentType, string reference);

        Task RemoveLogo(int organisationId);
    }
}
