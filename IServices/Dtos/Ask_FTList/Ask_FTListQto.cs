using ZKLT25.API.Helper;

namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 阀体型号查询条件DTO
    /// </summary>
    public class Ask_FTListQto : BasePageParams
    {
        /// <summary>
        /// 阀体型号关键字搜索（搜索型号和名称）
        /// </summary>
        public string? Keyword { get; set; }
        
        /// <summary>
        /// 是否外购筛选 (1=外购, 0=自制, null=全部)
        /// </summary>
        public int? isWG { get; set; }
        
        /// <summary>
        /// 是否询价筛选 (1=是, 0=否, null=全部)
        /// </summary>
        public int? isAsk { get; set; }
    }
}