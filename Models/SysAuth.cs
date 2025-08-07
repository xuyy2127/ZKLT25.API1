using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.Models
{
    public class SysAuth
    {
        [Key]
        public int Id { get; set; }
        public string? MenuID { get; set; }
        public string? PageID { get; set; }
        public int AuthOrder { get; set; }
        public string? UserName { get; set; }
        public DateTime SetDate { get; set; }
        public int? Favorites { get; set; }
    }
}
