using ZKLT25.API.Helper;

namespace ZKLT25.API.IServices.Dtos
{
    public class Ask_DataFJQto : BasePageParams
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
        /// 附件类型
        /// </summary>
        public string? OrdName { get; set; }

        /// <summary>
        /// 型号
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
        /// 数据源选择（-1=全部表，0=外部表，1=内部表）
        /// </summary>
        public int IsOutView { get; set; } = 1;
        
        /// <summary>
        /// 价格状态过滤（null=全部，true=已过期，false=未过期）
        /// </summary>
        public bool? IsExpired { get; set; }
    }
}