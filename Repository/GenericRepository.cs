using FINTCS.Data;
using FINTCS.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FINTCS.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _db;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }

        public async Task<T> GetFirstAsync(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
                return await _db.FirstOrDefaultAsync();

            return await _db.FirstOrDefaultAsync(predicate);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _db.FindAsync(id);
        }

        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate)
        {
            return await _db.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _db.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _db.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            _db.Update(entity);
        }

        public void Delete(T entity)
        {
            _db.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _db.RemoveRange(entities);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<TBLDEF>> GetDropdownListAsync()
        {
            return await _context.TBLDEF.OrderBy(x => x.DTBLDESC).ToListAsync();
        }
        public async Task<IEnumerable<T>> GetAll()
        {
            return await _db.ToListAsync();
        }

        public async Task Insert(T entity)
        {
            await _db.AddAsync(entity);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
        public async Task Add(T entity)
        {
            await _db.AddAsync(entity);
        }


    }

}

