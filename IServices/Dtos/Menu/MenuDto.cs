using ZKLT25.API.Models;

namespace ZKLT25.API.IServices.Dtos
{
    public class MenuDto
    {
        public string Id { get; set; }
        public string? ParentId { get; set; }
        public string? Name { get; set; }
        public string? Icon { get; set; }
        public string? MenuType { get; set; }
        public string? NavigateUrl { get; set; }
        public string? IframeUrl { get; set; }
        public string? Target { get; set; }
        public int? DeleteMark { get; set; }
        public string Favorites { get; set; } = "0";
        public int IsAuth { get; set; } = 0;
        public int? SortCode { get; set; }
        public List<MenuDto> Children { get; set; } = [];
    }
}
