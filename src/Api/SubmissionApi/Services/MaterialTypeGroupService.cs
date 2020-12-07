using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Common.Data;
using Biobanks.Common.Data.Entities.ReferenceData;
using Biobanks.SubmissionApi.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.SubmissionApi.Services
{
    /// <inheritdoc />
    public class MaterialTypeGroupService : IMaterialTypeGroupService
    {
        private readonly SubmissionsDbContext _db;

        /// <inheritdoc />
        public MaterialTypeGroupService(SubmissionsDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<MaterialTypeGroup>> List()
            => await _db.MaterialTypeGroups
                .Include(mtg => mtg.MaterialTypes)
                .ThenInclude(mtmtg => mtmtg.MaterialTypeGroups)
                .AsNoTracking()
                .ToListAsync();

        /// <inheritdoc />
        public async Task<MaterialTypeGroup> Get(int materialTypeGroupId)
            => await _db.MaterialTypeGroups
                .Include(mtg => mtg.MaterialTypes)
                .ThenInclude(mtmtg => mtmtg.MaterialTypeGroups)
                .FirstOrDefaultAsync(mtg => mtg.Id == materialTypeGroupId);

        /// <inheritdoc />
        public async Task<MaterialTypeGroup> GetByValue(string value)
            => await _db.MaterialTypeGroups.FirstOrDefaultAsync(mtg => mtg.Value == value);

        /// <inheritdoc />
        public async Task<MaterialTypeGroup> Create(MaterialTypeGroup materialTypeGroup)
        {
            await _db.MaterialTypeGroups.AddAsync(materialTypeGroup);
            await _db.SaveChangesAsync();
            return materialTypeGroup;
        }

        /// <inheritdoc />
        public async Task Update(MaterialTypeGroup materialTypeGroup)
        {
            _db.MaterialTypeGroups.Update(materialTypeGroup);
            await _db.SaveChangesAsync();
        }
    }
}