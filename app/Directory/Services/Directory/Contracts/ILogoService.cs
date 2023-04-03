using System.IO;
using System.Threading.Tasks;
using Biobanks.Data.Entities;

namespace Biobanks.Directory.Services.Directory.Contracts
{
    public interface ILogoService
    {
        Task<Blob> GetLogoBlob(string logoName);
        Task<string> StoreLogo(Stream logoFileStream, string logoFileName, string logoContentType, string reference);
        Task RemoveLogo(int organisationId);
    }
}
