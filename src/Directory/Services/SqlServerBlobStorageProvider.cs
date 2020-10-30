using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Directory.Entity.Data;
using Directory.Data.Repositories;
using Directory.Services.Contracts;

namespace Directory.Services
{
    public class SqlServerBlobStorageProvider : IBlobStorageProvider
    {
        private readonly IGenericEFRepository<Blob> _blobRepository;

        public SqlServerBlobStorageProvider(IGenericEFRepository<Blob> blobRepository)
        {
            _blobRepository = blobRepository;
        }

        public async Task<IEnumerable<Blob>> GetBlobsAsync(string resourceWildcard)
            => await _blobRepository.ListAsync(false, x => x.FileName.StartsWith(resourceWildcard));

        public async Task<Blob> GetBlobAsync(string resourceName)
        {
            var fileName = Path.GetFileNameWithoutExtension(resourceName);

            try
            {
                // Makes The Query Independent on File Extension
                return (await GetBlobsAsync(fileName))
                        .Where(x => x.FileName == resourceName || Path.GetFileNameWithoutExtension(x.FileName) == resourceName)
                        .First();
            }
            catch
            {
                throw new ApplicationException();
            }
        }

        public async Task<string> StoreBlobAsync(MemoryStream blob, string fileName, string contentType, string reference)
        {
            if (blob == null || blob.Length < 0)
                throw new ArgumentNullException(nameof(blob));

            var storeBlob = new Blob
            {
                //we include file extension in the resource name to ease contentdisposition later
                FileName = reference + Path.GetExtension(fileName),
                ContentType = contentType,
                ContentDisposition = "attachment; filename=" + reference + Path.GetExtension(fileName),
                Content = blob.ToArray()
            };

            //is there an existing logo blob for this biobank?
            //we want to replace, not keep adding and storing new files
            try
            {
                var existing = await GetBlobAsync(storeBlob.FileName);
                await _blobRepository.DeleteAsync(existing.Id);
            }
            catch (ApplicationException) { } //no worries if nothing found

            //write to db
            _blobRepository.Insert(storeBlob);
            await _blobRepository.SaveChangesAsync();

            return storeBlob.FileName;
        }

        public async Task DeleteBlobAsync(string resourceName)
        {
            await _blobRepository.DeleteWhereAsync(x => x.FileName == resourceName);
            await _blobRepository.SaveChangesAsync();
        }
    }
}
