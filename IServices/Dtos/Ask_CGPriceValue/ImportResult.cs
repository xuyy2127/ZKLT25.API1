namespace ZKLT25.API.IServices.Dtos
{
    public class ImportResult
    {
        /// <summary>
        /// 成功导入数量
        /// </summary>
        public int SuccessCount { get; set; }
        
        /// <summary>
        /// 失败数量
        /// </summary>
        public int FailCount { get; set; }
        
        /// <summary>
        /// 错误明细列表
        /// </summary>
        public List<ImportError> Errors { get; set; } = new List<ImportError>();
    }

    public class ImportError
    {
        /// <summary>
        /// 行号（从1开始）
        /// </summary>
        public int RowNumber { get; set; }
        
        /// <summary>
        /// 错误原因
        /// </summary>
        public string ErrorMessage { get; set; } = "";
    }
}
