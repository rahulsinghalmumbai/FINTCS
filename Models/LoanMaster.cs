namespace FINTCS.Models
{
    public class LoanMaster
    {
      
        public int LoanMasterId { get; set; } 

        public string? LoanName { get; set; } 

        public decimal? MultipleTimes { get; set; } = 0;

        public decimal? MaxLimit { get; set; } = 0;

        public decimal? LoanInt { get; set; } = 0;

        public int SocietyId { get; set; } = 0;
    }
}