using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FINTCS.Models
{
    [Table("Voucher")]
    public class Voucher
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("MastVoc")]
        public int KeyNo { get; set; }

        public int DbCr { get; set; }          // 1 = Dr , 2 = Cr

        public string ParticularCode { get; set; }

        public decimal Amount { get; set; }

        // 🔗 Navigation
        public MastVoc MastVoc { get; set; }
    }
}
