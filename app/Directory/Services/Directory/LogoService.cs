using System.Threading.Tasks;
using Biobanks.Data.Entities;
using Biobanks.Directory.Services.Directory.Contracts;

namespace Biobanks.Directory.Services.Directory
{
    public class LogoService: ILogoService
    {
        private readonly ILogoStorageProvider _logoStorageProvider;

        public LogoService(ILogoStorageProvider logoStorageProvider)
        {
            _logoStorageProvider = logoStorageProvider;
        }

        public async Task<Blob> GetLogoBlobAsync(string logoName)
        => await _logoStorageProvider.GetLogoBlobAsync(logoName);

    }
}
