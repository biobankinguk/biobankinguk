using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Common.Data;
using Biobanks.Common.Data.Entities.JoinEntities;
using Biobanks.Common.Data.Entities.ReferenceData;
using Biobanks.Common.Models;
using Biobanks.SubmissionApi.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.SubmissionApi.Services
{
    /// <inheritdoc />
    public class MaterialTypeService : IMaterialTypeService
    {
        private readonly SubmissionsDbContext _db;

        /// <inheritdoc />
        public MaterialTypeService(SubmissionsDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<MaterialType>> List()
            => await _db.MaterialTypes
                .Include(mt => mt.MaterialTypeMaterialTypeGroups)
                .ThenInclude(mtmtg => mtmtg.MaterialTypeGroup)
                .AsNoTracking()
                .ToListAsync();

        /// <inheritdoc />
        public async Task<MaterialType> Get(int materialTypeId)
            => await _db.MaterialTypes
                .Include(mt => mt.MaterialTypeMaterialTypeGroups)
                .ThenInclude(mtmtg => mtmtg.MaterialTypeGroup)
                .FirstOrDefaultAsync(mt => mt.Id == materialTypeId);

        /// <inheritdoc />
        public async Task<MaterialType> GetByValue(string value)
            => await _db.MaterialTypes.FirstOrDefaultAsync(mt => mt.Value == value);

        /// <inheritdoc />
        public async Task<MaterialType> Create(MaterialType materialType)
        {
            await _db.MaterialTypes.AddAsync(materialType);
            await _db.SaveChangesAsync();
            return materialType;
        }

        /// <inheritdoc />
        public async Task Create(IList<MaterialTypeJsonModel> materialTypes)
        {
            foreach (var materialType in materialTypes)
            {
                var materialTypeDbEntity = new MaterialType {Value = materialType.Value};

                foreach (var materialTypeGroup in materialType.MaterialTypeGroupNames)
                {
                    var materialTypeGroupDbEntity = _db.MaterialTypeGroups.FirstOrDefault(mtg => mtg.Value == materialTypeGroup);

                    if (materialTypeGroupDbEntity == null) continue;

                    if (materialTypeGroupDbEntity.MaterialTypeMaterialTypeGroups == null)
                        materialTypeGroupDbEntity.MaterialTypeMaterialTypeGroups =
                            new List<MaterialTypeMaterialTypeGroup>();

                    materialTypeGroupDbEntity?.MaterialTypeMaterialTypeGroups.Add(new MaterialTypeMaterialTypeGroup
                    {
                        MaterialType = materialTypeDbEntity,
                        MaterialTypeGroup = materialTypeGroupDbEntity
                    });
                }

                await _db.MaterialTypes.AddAsync(materialTypeDbEntity);

                await _db.SaveChangesAsync();
            }
        }

        /// <inheritdoc />
        public async Task Update(MaterialType materialType)
        {
            _db.MaterialTypes.Update(materialType);
            await _db.SaveChangesAsync();
        }
    }
}