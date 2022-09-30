using Biobanks.Entities.Data;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Biobanks.Services.Contracts
{
    [Obsolete("To be deleted when the Directory core version goes live." +
     " Any changes made here will need to be made in the corresponding core version"
     , false)]
    public interface ILogoStorageProvider
    {
        Task<Blob> GetLogoBlobAsync(string resourceName);

        Task<string> StoreLogoAsync(MemoryStream logo, string fileName, string contentType, string reference);

        Task RemoveLogoAsync(int organisationId);
    }
}
