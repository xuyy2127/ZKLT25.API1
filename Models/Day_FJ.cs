using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZKLT25.API.Models
{
    [Table("Day_FJ")]
    public class Day_FJ
    {
        [Key]
        public int id { get; set; }

        [StringLength(500)]
        public string? version { get; set; }

        [StringLength(50)]
        public string? Sup { get; set; }

        [StringLength(50)]
        public string? N1 { get; set; }

        [StringLength(50)]
        public string? N2 { get; set; }

        [StringLength(50)]
        public string? Day { get; set; }

        [StringLength(50)]
        public string? type { get; set; }

        public DateTime? DoDate { get; set; }

        [StringLength(50)]
        public string? DoUser { get; set; }
    }
}
