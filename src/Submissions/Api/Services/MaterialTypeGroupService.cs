using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Api.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Biobanks.Data;

namespace Biobanks.Submissions.Api.Services
{
    /// <inheritdoc />
    public class MaterialTypeGroupService : IMaterialTypeGroupService
    {
        private readonly BiobanksDbContext _db;

        /// <inheritdoc />
        public MaterialTypeGroupService(BiobanksDbContext db)
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