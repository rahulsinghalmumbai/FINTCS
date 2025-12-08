using FINTCS.Models;
using System.Linq.Expressions;

namespace FINTCS.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetFirstAsync(Expression<Func<T, bool>> predicate = null);
        Task<T> GetByIdAsync(int id);

        Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        void Update(T entity);

        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);

        Task SaveAsync();
        Task<IEnumerable<TBLDEF>> GetDropdownListAsync();
        Task<IEnumerable<T>> GetAll();   
        Task Add(T entity);
        Task Insert(T entity);
        Task Save();

    }
}
