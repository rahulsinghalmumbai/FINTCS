using System.ComponentModel.DataAnnotations;

namespace FINTCS.Models
{
    public class TBLDAT
    {
        [Key]
        public int TBLSSERN { get; set; }

        public int DTBLSERN { get; set; }
        [Required(ErrorMessage = "Description is required")]
        [StringLength(200)]
        public string? TBLSDESC { get; set; } = string.Empty;
    }
}
