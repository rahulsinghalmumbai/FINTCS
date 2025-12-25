namespace FINTCS.ViewModels
{
    public class VoucherVM
    {
        public int KeyNo { get; set; }
        public int VoucherType { get; set; }
        public DateTime VoucherDate { get; set; }

        public int SerialNo { get; set; }

        public string ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }

        public string Narration { get; set; }
        public string Remarks { get; set; }
        public DateTime PassedDate { get; set; }

        public List<VoucherItemVM> Items { get; set; } = new();
    }

}
