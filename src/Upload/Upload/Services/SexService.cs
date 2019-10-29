using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.SubmissionApi.Services.Contracts;
using Common.Data;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.SubmissionApi.Services
{
    /// <inheritdoc />
    public class SexService : ISexService
    {
        private readonly UploadContext _db;

        /// <inheritdoc />
        public SexService(UploadContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Sex>> List()
        {
            throw new NotImplemented();
            //Needs to call Ref Data API
            //  => await _db.Sexes.AsNoTracking().ToListAsync();
        }


        /// <inheritdoc />
        public async Task<Sex> Get(int sexId)
            => await _db.Sexes.FirstOrDefaultAsync(s => s.Id == sexId);


    }
}