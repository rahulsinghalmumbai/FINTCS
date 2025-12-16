using FINTCS.Models;
using System.Linq.Expressions;

namespace FINTCS.Repositories
{
    public interface ISocietyService<T> where T : class
    {
        Task<T?> GetFirstAsync(Expression<Func<T, bool>>? predicate = null);


        Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate);

      
        
        Task<IEnumerable<TBLDEF>> GetDropdownListAsync();
     
        Task<int> UpsertSocietyAsync(Society society);
        Task<int> UpsertLoanMasterAsync(LoanMaster loan);
        Task<int> UpsertTBLDATAsync(TBLDAT data);
    }
}
