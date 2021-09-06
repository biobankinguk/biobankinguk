﻿using AutoMapper.Configuration.Annotations;
using Biobanks.Directory.Data;
using Biobanks.Directory.Services.Contracts;
using Biobanks.Entities.Data.ReferenceData;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class ReferenceDataService<T> : IReferenceDataService<T> where T : ReferenceDataBase
    {
        private readonly BiobanksDbContext _db;

        public ReferenceDataService(BiobanksDbContext db)
        {
            _db = db;
        }

        protected virtual IQueryable<T> Query()
            => _db.Set<T>().AsQueryable();

        /// <inheritdoc/>
        public async Task Add(T entity)
        {
            _db.Set<T>().Add(entity);
            await _db.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<int> Count()
            => await Query().CountAsync();

        /// <inheritdoc/>
        public async Task Delete(int id)
        {
            // Instantiate Generic
            var entity = Activator.CreateInstance<T>();
            entity.Id = id;

            // Link and Remove Entity By Reference
            _db.Set<T>().Attach(entity);
            _db.Set<T>().Remove(entity);

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> Exists(string value)
            => await Query().AnyAsync(x => x.Value == value);

        /// <inheritdoc/>
        public async Task<T> Get(int id)
            => await Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

        /// <inheritdoc/>
        public async Task<ICollection<T>> List()
            => await Query()
                .AsNoTracking()
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<T> Update(T entity)
        {
            var exisiting = await Query().FirstOrDefaultAsync(x => x.Id == entity.Id);

            if (exisiting is null)
                throw new KeyNotFoundException($"No exisiting { nameof(T) } for given Id={ entity.Id }");

            // Update Properties
            exisiting.SortOrder = entity.SortOrder;
            exisiting.Value = entity.Value;

            // Re-Order

            // Save Changes
            await _db.SaveChangesAsync();

            return exisiting;
        }
    }

    public class AccessConditionService : ReferenceDataService<AccessCondition>
    {
        public AccessConditionService(BiobanksDbContext db) : base(db) { }

        protected override IQueryable<AccessCondition> Query()
            => base.Query().Include(x => x.Organisations);
    }
}
