using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory
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
