using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZKLT25.API.Models
{
    [Table("Favorites")]
    public class Favorite
    {
        [Key]
        public int? Id {  get; set; }
        public string? UserName { get;set; }
        public string? Favorites { get; set; }
    }
}
