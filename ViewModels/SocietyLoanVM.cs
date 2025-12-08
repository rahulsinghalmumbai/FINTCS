using FINTCS.Models;

namespace FINTCS.ViewModels
{
    public class SocietyLoanVM
    {
        public Society Society { get; set; } = new Society();

        public List<LoanMaster> LoanMasters { get; set; } = new List<LoanMaster>();

        public List<LoanMaster> LoanList { get; set; } = new();
    }
}
