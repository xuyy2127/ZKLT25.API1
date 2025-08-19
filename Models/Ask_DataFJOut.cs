using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZKLT25.API.Models
{
    /// <summary>
    /// 询价附件数据外部表
    /// </summary>
    [Table("Ask_DataFJOut")]
    public class Ask_DataFJOut
    {
        [Key]
        public int ID { get; set; }
        
        /// <summary>
        /// 来源
        /// </summary>
        [StringLength(100)]
        public string? Source { get; set; } = "询价待办";
        
        /// <summary>
        /// 询价日期
        /// </summary>
        public DateTime? AskDate { get; set; }
        
        /// <summary>
        /// 询价项目名称
        /// </summary>
        [StringLength(500)]
        public string? AskProjName { get; set; }
        
        /// <summary>
        /// 附件类型
        /// </summary>
        [StringLength(100)]
        public string? FJType { get; set; }
        
        /// <summary>
        /// 附件版本
        /// </summary>
        public string? FJVersion { get; set; }
        
        /// <summary>
        /// 单位
        /// </summary>
        [StringLength(100)]
        public string? Unit { get; set; }
        
        /// <summary>
        /// 数量
        /// </summary>
        public int? Num { get; set; }
        
        /// <summary>
        /// 价格
        /// </summary>
        public double? Price { get; set; }
        
        /// <summary>
        /// 供应商
        /// </summary>
        [StringLength(100)]
        public string? Supplier { get; set; }
        
        /// <summary>
        /// 项目天数
        /// </summary>
        [StringLength(100)]
        public string? ProjDay { get; set; }
        
        /// <summary>
        /// 天数1
        /// </summary>
        [StringLength(100)]
        public string? Day1 { get; set; }
        
        /// <summary>
        /// 天数2
        /// </summary>
        [StringLength(100)]
        public string? Day2 { get; set; }
        
        /// <summary>
        /// 天数3
        /// </summary>
        [StringLength(100)]
        public string? Day3 { get; set; }
        
        /// <summary>
        /// 备注1
        /// </summary>
        [StringLength(1000)]
        public string? Memo1 { get; set; }
        
        /// <summary>
        /// 执行用户
        /// </summary>
        [StringLength(100)]
        public string? DoUser { get; set; }
        
        /// <summary>
        /// 执行日期
        /// </summary>
        public DateTime? DoDate { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(1000)]
        public string? Memo { get; set; }
        
        /// <summary>
        /// 价格比例
        /// </summary>
        public double? PriceRatio { get; set; }
        
        /// <summary>
        /// 账单详情ID
        /// </summary>
        public int? BillDetailID { get; set; }
        
        /// <summary>
        /// 通径
        /// </summary>
        [StringLength(200)]
        public string? DN { get; set; }
        
        /// <summary>
        /// 压力
        /// </summary>
        [StringLength(200)]
        public string? PN { get; set; }
        
        /// <summary>
        /// 连接标准
        /// </summary>
        [StringLength(200)]
        public string? OrdLJ { get; set; }
        
        /// <summary>
        /// 是否预生产绑定
        /// </summary>
        public int IsPreProBind { get; set; } = 0;
        
        /// <summary>
        /// 超时
        /// </summary>
        public int? Timeout { get; set; } = -1;
    }
}
