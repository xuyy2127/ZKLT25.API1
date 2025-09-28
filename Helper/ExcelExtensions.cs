using Aspose.Cells;

namespace ZKLT25.API.Helper
{
    public static class ExcelExtensions
    {
        public static string? GetString(this Worksheet worksheet, int row, int col)
        {
            var v = worksheet.Cells[row, col].Value;
            return v?.ToString()?.Trim();
        }
    }
}


