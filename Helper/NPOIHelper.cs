using NPOI.SS.UserModel;

namespace ZKLT25.API.Helper
{
    public static class NPOIHelper
    {
        /// <summary>
        /// 在ISheet中插入内容
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="cellVals"></param>
        public static void SetCellVal(this ISheet sheet, List<(int, int, string)> cellVals)
        {
            var rows = cellVals.Select(x => x.Item1).Distinct().OrderBy(x => x).ToList();
            foreach (var rowIndex in rows)
            {
                IRow row = sheet.GetRow(rowIndex);

                var cells = cellVals.Where(x => x.Item1 == rowIndex).ToList();

                ICell cell = null;
                foreach (var item in cells)
                {
                    cell = row.GetCell(item.Item2);
                    cell.SetCellValue(item.Item3);
                }
            }
        }
    }
}
