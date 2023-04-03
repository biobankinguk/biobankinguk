using System.IO;
using System.Threading.Tasks;
using Biobanks.Data.Entities;
using Biobanks.Directory.Services.Directory.Contracts;
using Biobanks.Directory.Services.Submissions;

namespace Biobanks.Directory.Services.Directory
{
  public class LogoService : ILogoService
  {
    private readonly ILogoStorageProvider _logoStorageProvider;

    public LogoService(ILogoStorageProvider logoStorageProvider)
    {
      _logoStorageProvider = logoStorageProvider;
    }

    public async Task<Blob> GetLogoBlob(string logoName)
      => await _logoStorageProvider.GetLogoBlob(logoName);

    public async Task<string> StoreLogo(Stream logoFileStream, string logoFileName, string logoContentType,
      string reference)
    {
      var resizedLogoStream = await ImageService.ResizeImageStream(logoFileStream, maxX: 300, maxY: 300);

      return await _logoStorageProvider.StoreLogo(resizedLogoStream, logoFileName, logoContentType, reference);
    }

    public async Task RemoveLogo(int organisationId)
    {
      await _logoStorageProvider.RemoveLogo(organisationId);
    }
  }
}
