using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Directory.Entity.Data;
using Directory.Data.Repositories;
using Directory.Services.Contracts;

namespace Directory.Services
{
    public class SqlServerLogoStorageProvider : ILogoStorageProvider
    {
        private readonly IGenericEFRepository<Blob> _blobRepository;
        private readonly IGenericEFRepository<Organisation> _organisationRepository;

        public SqlServerLogoStorageProvider(
            IGenericEFRepository<Blob> blobRepository,
            IGenericEFRepository<Organisation> organisationRepository)
        {
            _blobRepository = blobRepository;
            _organisationRepository = organisationRepository;
        }

        public async Task<Blob> GetLogoBlobAsync(string resourceName)
        {
            var blob = (await _blobRepository.ListAsync(
                false,
                x => x.FileName == resourceName))
                .FirstOrDefault();

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
            try
            {
                var existing = await GetLogoBlobAsync(logoBlob.FileName);
                await _blobRepository.DeleteAsync(existing.Id);
            } catch (ApplicationException) { } //no worries if nothing found

            //write to db
            _blobRepository.Insert(logoBlob);
            await _blobRepository.SaveChangesAsync();

            return logoBlob.FileName;
        }

        public async Task RemoveLogoAsync(int organisationId)
        {
            var organisation = await _organisationRepository.GetByIdAsync(organisationId);

            if (organisation != null)
                await _blobRepository.DeleteWhereAsync(x => x.FileName == organisation.Logo);
        }
    }
}
