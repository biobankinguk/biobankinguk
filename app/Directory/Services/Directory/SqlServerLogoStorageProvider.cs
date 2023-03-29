using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities;
using Biobanks.Directory.Services.Directory.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
{
    public class SqlServerLogoStorageProvider : ILogoStorageProvider
    {
      private readonly ApplicationDbContext _db;

      public SqlServerLogoStorageProvider(
          ApplicationDbContext db)
        {
          _db = db;
        }

        public async Task<Blob> GetLogoBlobAsync(string resourceName)
        {
        
            var blob = await _db.Blobs
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
              _db.Remove(existing.Id);

            //write to db
            _db.Add(logoBlob);
            await _db.SaveChangesAsync();

            return logoBlob.FileName;
        }

        public async Task RemoveLogoAsync(int organisationId)
        {
            var organisation = await _db.FindAsync<Organisation>(organisationId);
            var entity = _db.Blobs.Where(x => x.FileName == organisation.Logo);
            if (organisation != null)
                _db.Remove(entity);
             
            await _db.SaveChangesAsync();
    }
  }
}
