﻿using Biobanks.Directory.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    [Obsolete("To be deleted when the Directory core version goes live." +
        " Any changes made here will need to be made in the corresponding core version"
        , false)]
    public class AssociatedDataTypeService : ReferenceDataService<AssociatedDataType>
    {
        public AssociatedDataTypeService(BiobanksDbContext db) : base(db) { }

        protected override IQueryable<AssociatedDataType> Query()
            => base.Query().Include(x => x.AssociatedDataTypeGroup);

        public override async Task<int> GetUsageCount(int id)
            => await _db.CollectionAssociatedDatas.CountAsync(x => x.AssociatedDataTypeId == id)
             + await _db.CapabilityAssociatedDatas.CountAsync(x => x.AssociatedDataTypeId == id);
        
        public override async Task<bool> IsInUse(int id)
            => await _db.CollectionAssociatedDatas.AnyAsync(x => x.AssociatedDataTypeId == id)
            || await _db.CapabilityAssociatedDatas.AnyAsync(x => x.AssociatedDataTypeId == id);

        public override async Task<AssociatedDataType> Update(AssociatedDataType entity)
        {
            var existing = await this.Get(entity.Id);
            existing.AssociatedDataTypeGroup = entity.AssociatedDataTypeGroup;
            existing.AssociatedDataTypeGroupId = entity.AssociatedDataTypeGroupId;
            existing.Message = entity.Message;
            existing.OntologyTerms = entity.OntologyTerms;
            await _db.SaveChangesAsync();
            return existing;
        }

        public override async Task<ICollection<AssociatedDataType>> List(string wildcard)
            => await _db.AssociatedDataTypes
                .AsNoTracking()
                .Include(x => x.OntologyTerms)
                .Where(x => x.Value.Contains(wildcard))
                .OrderBy(x => x.SortOrder)
                .ToListAsync();


        public override async Task<AssociatedDataType> Get(int id)
        {
            var list = await _db.AssociatedDataTypes
                .Include(x => x.OntologyTerms)
                .Where(x => x.Id == id)
                .ToListAsync();
            if (list.Count > 0)
            {
                return list[0];
            }
            return null;
            
        }


    }
}