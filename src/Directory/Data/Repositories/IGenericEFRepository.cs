using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Biobanks.Directory.Data.Repositories
{
    public interface IGenericEFRepository<TEntity>
    {
        Task<IEnumerable<TEntity>> ListAsync(bool tracking = false,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProperties);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null);

        IEnumerable<TEntity> List(bool tracking = false,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProperties);

        Task<TEntity> GetByIdAsync(object id);

        void Insert(TEntity entity);

        Task DeleteAsync(object id);

        void Delete(TEntity entityToDelete);

        Task DeleteWhereAsync(Expression<Func<TEntity, bool>> filter = null);

        void Update(TEntity entityToUpdate);

        Task<IEnumerable<TEntity>> GetWithRawSqlAsync(string query, params object[] parameters);

        Task<int> SaveChangesAsync();
    }
}
