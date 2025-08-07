namespace ZKLT25.API.IServices.Dtos
{
    public class MenuUto
    {
        public int? Method { get; set; }
        public string? MenuName { get; set; }
        public int? DeleteMark { get; set; }
        public string MenuType { get; set; }
        public string? MenuImg { get; set; }
        public string? IframeUrl { get; set; }
        public string? NavigateUrl { get; set; }
        public string? Target { get; set; }
        public int? SortCode { get; set; }
        public string? ParentId { get; set; }
    }
}
