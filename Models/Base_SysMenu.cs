using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.Models
{
    public class Base_SysMenu
    {
        [Key]
        public string Id { get; set; }
        public string? ParentId { get; set; }
        public string? Menu_Name { get; set; }
        public string? Menu_Img { get; set; }
        public string? Menu_Type { get; set; }
        public string? NavigateUrl { get; set; }
        public string? Target { get; set; }
        public string? IframeUrl { get; set; }
        public int SortCode { get; set; }
        public int? DeleteMark { get; set; }
        public string? CreateUserName { get; set; }
        public DateTime? CreateDate { get; set; }
        public int State { get; set; }
    }
}
