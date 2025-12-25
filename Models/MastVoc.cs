using FINTCS.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FINTCS.Models
{
    [Table("Mast_Voc")]
    public class MastVoc
    {
        [Key]
        public int KeyNo { get; set; }

        public int VoucherType { get; set; }   // 5-Receipt,6-Payment,7-Journal,8-Contra

        public int SerialNo { get; set; }

        [DataType(DataType.Date)]
        public DateTime VoucherDate { get; set; }

        public string? ChequeNo { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ChequeDate { get; set; }

        public string? Narration { get; set; }

        public string? Remarks { get; set; }

        [DataType(DataType.Date)]
        public DateTime PassedDate { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        // 🔗 Navigation
        public ICollection<Voucher> Vouchers { get; set; }
    }
}

