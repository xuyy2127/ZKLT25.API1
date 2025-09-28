using ZKLT25.API.Helper;

namespace ZKLT25.API.IServices.Dtos
{
    public class Ask_BillQto: BasePageParams
    {
        /// <summary>
        /// ID
        /// </summary>
        public int? BillID { get; set; }
        
        /// <summary>
        /// 询价单据关键字搜索
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>
        /// 类型筛选 FT=阀体
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// 查询开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 查询结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
}