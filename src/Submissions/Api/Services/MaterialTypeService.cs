﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Core.Models;
using Biobanks.Submissions.Api.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Biobanks.Data;

namespace Biobanks.Submissions.Api.Services
{
    /// <inheritdoc />
    public class MaterialTypeService : IMaterialTypeService
    {
        private readonly BiobanksDbContext _db;

        /// <inheritdoc />
        public MaterialTypeService(BiobanksDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<MaterialType>> List()
            => await _db.MaterialTypes
                .Include(mt => mt.MaterialTypeGroups)
                .ThenInclude(mtmtg => mtmtg.MaterialTypes)
                .AsNoTracking()
                .ToListAsync();

        /// <inheritdoc />
        public async Task<MaterialType> Get(int materialTypeId)
            => await _db.MaterialTypes
                .Include(mt => mt.MaterialTypeGroups)
                .ThenInclude(mtmtg => mtmtg.MaterialTypes)
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

                    if (materialTypeGroupDbEntity.MaterialTypes == null)
                        materialTypeGroupDbEntity.MaterialTypes =
                            new List<MaterialType>();

                    materialTypeGroupDbEntity?.MaterialTypes.Add(new MaterialType
                    {
                        Id = materialTypeDbEntity.Id,
                        Value = materialTypeDbEntity.Value,
                        MaterialTypeGroups = materialTypeDbEntity.MaterialTypeGroups
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