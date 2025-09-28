using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.Models
{
    public class Ask_Bill
    {
        [Key]
        public int BillID { get; set; }
        
        /// <summary>
        /// 项目名称
        /// </summary>
        public string? ProjName { get; set; }
        
        /// <summary>
        /// 项目用户
        /// </summary>
        public string? ProjUser { get; set; }
        
        /// <summary>
        /// 文档名称
        /// </summary>
        public string? DocName { get; set; }
        
        /// <summary>
        /// 开票用户
        /// </summary>
        public string? KUser { get; set; }
        
        /// <summary>
        /// 开票日期
        /// </summary>
        public DateTime? KDate { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 供应商ID
        /// </summary>
        public int? SuppID { get; set; }
        
        /// <summary>
        /// 询价单状态
        /// </summary>
        public int? BillState { get; set; }
        
        /// <summary>
        /// 价格账单ID
        /// </summary>
        public int? PriceBillID { get; set; }
        
        /// <summary>
        /// 项目
        /// </summary>
        public string? Proj { get; set; }
        
        /// <summary>
        /// 执行用户
        /// </summary>
        public string? DoUser { get; set; }
        
        /// <summary>
        /// 预计回复日期
        /// </summary>
        public DateTime? YSDate { get; set; }
        
        /// <summary>
        /// 原因
        /// </summary>
        public string? Reason { get; set; }
        
        /// <summary>
        /// 询价单类型
        /// </summary>
        public int? BillType { get; set; } = 0;
        
        /// <summary>
        /// 关闭备注
        /// </summary>
        public string? ShutMemo { get; set; }
    }
}
