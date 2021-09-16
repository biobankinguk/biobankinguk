﻿using Biobanks.Directory.Data;
using Biobanks.Directory.Services.Contracts;
using Biobanks.Entities.Data.ReferenceData;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public abstract class ReferenceDataService<T> : IReferenceDataService<T> where T : BaseReferenceData
    {
        protected readonly BiobanksDbContext _db;

        public ReferenceDataService(BiobanksDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc/>
        public abstract Task<int> GetUsageCount(int id);

        /// <inheritdoc/>
        public abstract Task<bool> IsInUse(int id);

        /// <summary>
        /// Exposes an IQueryable to the underlying database table. Called upon by all service methods to access
        /// the Entity store.
        /// </summary>
        /// <returns>An IQueryable for the given Entity type <typeparamref name="T"/></returns>
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
        public async Task<bool> Exists(int id)
            => await Query().AnyAsync(x => x.Id == id);

        /// <inheritdoc/>
        public async Task<bool> Exists(string value)
            => await Query().AnyAsync(x => x.Value == value);

        /// <inheritdoc/>
        public async Task<T> Get(int id)
            => await Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

        /// <inheritdoc/>
        public async Task<T> Get(string value)
            => await Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Value == value);

        /// <inheritdoc/>
        public async Task<ICollection<T>> List()
            => await Query()
                .AsNoTracking()
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<T> Update(T entity)
        {
            var existing = await Query().FirstOrDefaultAsync(x => x.Id == entity.Id);

            if (existing is null)
                throw new KeyNotFoundException($"No exisiting { nameof(T) } for given Id={ entity.Id }");

            // Update Properties
            existing.SortOrder = entity.SortOrder;
            existing.Value = entity.Value;

            // Re-Order If Necessary
            if (existing.SortOrder != default)
            {
                var sortable = await Query()
                    .Where(x => x.Id != existing.Id)
                    .Where(x => x.SortOrder >= existing.SortOrder)
                    .OrderBy(x => x.SortOrder)
                    .ToListAsync();

                // Update SortOrder Indicies
                for (int i=0; i < sortable.Count; i++)
                {
                    sortable[i].SortOrder = existing.SortOrder + i + 1;
                }
            }

            // Save Changes
            await _db.SaveChangesAsync();

            return existing;
        }

    }
}