using FINTCS.Areas.Members.Models;
using FINTCS.Models;

namespace FINTCS.Repositories
{
    public interface IMemberService
    {
        Task<Member?> GetByIdAsync(int id);
        Task<int> AddOrUpdateStepAsync(Member model, int step);
        Task<List<TBLDAT>> GetBranchesAsync();        // DTBLSERN = 3
        Task<List<TBLDAT>> GetDesignationsAsync();   // DTBLSERN = 1
        Task<List<TBLDAT>> GetNomineeRelationsAsync(); // DTBLSERN = 4
    }
}
