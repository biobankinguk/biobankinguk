using Biobanks.Data;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services
{
    public class GenericEFRepository<TEntity> : IGenericEFRepository<TEntity> where TEntity : class
    {
        private readonly BiobanksDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericEFRepository(BiobanksDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> ListAsync(bool tracking = false,
                                               Expression<Func<TEntity, bool>> filter = null,
                                               Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                               params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = tracking ? _dbSet : _dbSet.AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            if (orderBy != null)
            {
                return await orderBy.Invoke(query).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            return filter == null
                ? await _dbSet.CountAsync()
                : await _dbSet.Where(filter).CountAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            return filter == null
                ? await _dbSet.AnyAsync()
                : await _dbSet.Where(filter).AnyAsync();
        }

        public IEnumerable<TEntity> List(bool tracking = false,
                                    Expression<Func<TEntity, bool>> filter = null,
                                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                    params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            if (orderBy != null)
            {
                return orderBy.Invoke(query).ToList();
            }

            return query.ToList();
        }

        public async Task<TEntity> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public async Task DeleteAsync(object id)
        {
            var entityToDelete = await _dbSet.FindAsync(id);
            Delete(entityToDelete);
        }

        public void Delete(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        public async Task DeleteWhereAsync(Expression<Func<TEntity, bool>> filter)
        {
            if (filter == null) return;

            foreach (var entity in await ListAsync(tracking: true, filter: filter))
            {
                _dbSet.Remove(entity);
            }
        }

        /// <summary>
        /// This method is DANGEROUS as it can attempt to attach already
        /// tracked entities if you're not VERY CAREFUL.
        /// 
        /// Recommend not to use it and instead:
        /// 
        /// 1. retrieve a tracked version of the entity to update
        /// 2. manually update properties or use automapper to apply dto to the tracked entity
        /// 3. save changes without calling this method.
        /// 
        /// If using automapper, remember to add a mapping profile if necessary.
        /// You can map from the same type to itself if you need to.
        /// 
        /// That should get you out of any remaining problem use of this method.
        /// </summary>
        /// <param name="entityToUpdate"></param>
        [Obsolete]
        public void Update(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public async Task<IEnumerable<TEntity>> GetWithRawSqlAsync(string query, params object[] parameters)
        {
            return await _dbSet.FromSqlRaw(query, parameters).ToListAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
