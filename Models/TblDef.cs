using System.ComponentModel.DataAnnotations;

namespace FINTCS.Models
{
    public class TBLDEF
    {
        [Key]
        public int DTBLSERN { get; set; }
        public string? DTBLDESC { get; set; } = "";
    }
}
