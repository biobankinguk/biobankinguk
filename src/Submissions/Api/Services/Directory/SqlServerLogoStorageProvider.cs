using Biobanks.Data;
using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class SqlServerLogoStorageProvider : ILogoStorageProvider
    {
      private readonly ApplicationDbContext _context;

      public SqlServerLogoStorageProvider(
          ApplicationDbContext context)
        {
          _context = context;
        }

        public async Task<Blob> GetLogoBlobAsync(string resourceName)
        {
        
            var blob = await _context.Blobs
                .AsNoTracking()
                .Where(x => x.FileName == resourceName)
                .FirstOrDefaultAsync();

            if (blob == null) throw new ApplicationException();

            return blob;
        }

        public async Task<string> StoreLogoAsync(MemoryStream logo, string fileName, string contentType, string reference)
        {
            if (logo == null || logo.Length <= 0)
                throw new ArgumentNullException(nameof(logo));

            var logoBlob = new Blob
            {
                //we include file extension in the resource name to ease contentdisposition later
                FileName = reference + Path.GetExtension(fileName),
                ContentType = contentType,
                ContentDisposition = "attachment; filename=" + reference + Path.GetExtension(fileName),
                Content = logo.ToArray()
            };

            //is there an existing logo blob for this biobank?
            //we want to replace, not keep adding and storing new files
     
            var existing = await GetLogoBlobAsync(logoBlob.FileName);
              _context.Remove(existing.Id);

            //write to db
            _context.Add(logoBlob);
            await _context.SaveChangesAsync();

            return logoBlob.FileName;
        }

        public async Task RemoveLogoAsync(int organisationId)
        {
            var organisation = await _context.FindAsync<Organisation>(organisationId);
            var entity = _context.Blobs.Where(x => x.FileName == organisation.Logo);
            if (organisation != null)
                _context.Remove(entity);
             
            await _context.SaveChangesAsync();
    }
  }
}
