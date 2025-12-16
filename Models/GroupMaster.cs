namespace FINTCS.Models
{
    public class GroupMaster
    {
        public int Id { get; set; }
        public string? GroupName { get; set; }
        public int? UnderGroupId { get; set; }
        public string? NatureOfGroup { get; set; }
        public string? DCode { get; set; }
        public int LedgerCounter { get; set; }
        public int? LedgerCount { get; set; }


    }

}
