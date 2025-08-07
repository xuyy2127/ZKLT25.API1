using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.Models
{
    public class SysDepart
    {

        [Key]
        public string DepartID { get; set; }
        public string? DepartName { get; set; }
        public string? DepartState { get; set; }
        public int DepartOrder { get; set; }
    }
}
