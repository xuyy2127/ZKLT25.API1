namespace ZKLT25.API.IServices.Dtos
{
    public class MaterialComparisonDto
    {
        public string? Type { get; set; }
        public string? Name { get; set; }
        public string? Version { get; set; }
        public string? DN { get; set; }
        public string? PN { get; set; }
        public string? FT { get; set; }
        public string? GroupType { get; set; }
        public string? IsWGText { get; set; }
        public double CurrentNum { get; set; }
        public double HistoryNum { get; set; }
        public double ChangeNum { get; set; }
        public string? ChangeType { get; set; }
    }
}


