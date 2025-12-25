namespace FINTCS.DTOs
{
    public class LedgerListDto
    {
        public int Id { get; set; }
        public string? ledger { get; set; }
        public int GroupId { get; set; }
        public string? GroupName { get; set; }
        public decimal? OpeningBalance { get; set; }
        public int? drcr { get; set; }
        public bool? Bank { get; set; }



    }
}
