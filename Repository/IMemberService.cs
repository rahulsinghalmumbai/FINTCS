using FINTCS.Areas.Members.Models;
using FINTCS.Areas.Members.ViewModel;
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
        

        Task GenerateMemberSpecificLedgersAsync(int memberId, string memNo, string memberName);

        Task UpdateMemberLedgerNamesAsync(int memberId, string memNo, string memberName);
        bool IsMemNoExists(string memno, int id);
        Task<List<LoanAmountVM>> GetLoanMasterAsync(int memberId);
        Task SaveLoanAmountsAsync(int memberId, List<LoanAmountVM> loans);
        Task<List<MemberSearchVM>> GetMembersAsync(string search);
        
    }
}
