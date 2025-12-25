using System.ComponentModel.DataAnnotations.Schema;

namespace FINTCS.Areas.Members.ViewModel
{
    [NotMapped]
    public class LoanAmountVM
    {
        public int? LoanId { get; set; }
        public string? LoanName { get; set; }
        public decimal? Amt { get; set; }
    }
}
