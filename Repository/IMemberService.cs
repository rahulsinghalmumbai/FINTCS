using FINTCS.Areas.Members.Models;

namespace FINTCS.Repositories
{
    public interface IMemberService
    {
        Task<Member?> GetByIdAsync(int id);
        Task<int> AddOrUpdateStepAsync(Member model, int step);
    }
}
