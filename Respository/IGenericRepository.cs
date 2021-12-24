using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repository
{
    public interface IGenericRepository<TEntity> where TEntity : Item
    {
        void Delete(TEntity entityToDelete);
        Task DeleteAsync(object id);
        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "");
        Task<TEntity> GetByIdAsync(object id);
        Task<TEntity> InsertAsync(TEntity entity);
        void Save();
        void Update(TEntity entityToUpdate);
    }
}