namespace FINTCS.ViewModels
{
    public class VoucherItemVM
    {
        public int DbCr { get; set; }       // 1 Dr | 2 Cr
        public string? ParticularCode { get; set; }
        public decimal Amount { get; set; }
        public string ?ParticularName { get; set; }
    }
}
