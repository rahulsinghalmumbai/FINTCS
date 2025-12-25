namespace FINTCS.Areas.Members.Models
{
    public class LoanMasterMember
    {
        public int Id { get; set; }

        public int? MemberId { get; set; }
        public int? LoanId { get; set; }
        public decimal? Amt { get; set; }


    }
}
