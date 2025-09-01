using ZKLT25.API.Helper;

namespace ZKLT25.API.IServices.Dtos
{
    public class Ask_DataFJQto : BasePageParams, IPriceDataQto
    {
        /// <summary>
        /// 交期编号
        /// </summary>
        public int? BillID { get; set; }
        
        /// <summary>
        /// 项目名称
        /// </summary>
        public string? AskProjName { get; set; }
        
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string? SuppName { get; set; }
        /// <summary>
        /// 供应商ID
        /// </summary>
        public int? SuppID { get; set; }

        /// <summary>
        /// 附件名称
        /// </summary>
        public string? FJType { get; set; }
        
        /// <summary>
        /// 附件（FJTypeId）
        /// </summary>
        public int? FJTypeId { get; set; }

        /// <summary>
        /// 附件型号
        /// </summary>
        public string? FJVersion { get; set; }
        
        /// <summary>
        /// 查询开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }
        
        /// <summary>
        /// 查询结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }
        
        /// <summary>
        /// 价格状态过滤（null=全部，true=已过期，false=未过期）
        /// </summary>
        public bool? IsExpired { get; set; }
    }
}