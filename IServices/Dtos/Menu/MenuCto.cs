namespace ZKLT25.API.IServices.Dtos
{
    public class MenuCto
    {
        public int Method { get; set; }
        public string? MenuId { get; set; }
        public string Menu_Name { get; set; }
        public int DeleteMark { get; set; }
        public string Menu_Type { get; set; }
        public string? Menu_Img { get; set; }
        public string? IframeUrl { get; set; }
        public string? NavigateUrl { get; set; }
        public string? Target { get; set; }
        public int? SortCode { get; set; } = 0;
        public string ParentId { get; set; } = "0";
        public int? State { get; set; } = 0;
        public string Version { get; set; } = "V1.0.0";
    }
}
