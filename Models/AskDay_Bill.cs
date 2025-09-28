using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZKLT25.API.Models
{
    /// <summary>
    /// 交期单据表
    /// </summary>
    [Table("AskDay_Bill")]
    public class AskDay_Bill
    {
        [Key]
        public int BillID { get; set; }
        
        /// <summary>
        /// 项目名称
        /// </summary>
        [StringLength(200)]
        public string? ProjName { get; set; }
        
        /// <summary>
        /// 项目负责人
        /// </summary>
        [StringLength(200)]
        public string? ProjUser { get; set; }
        
        /// <summary>
        /// 文档名称
        /// </summary>
        [StringLength(200)]
        public string? DocName { get; set; }
        
        /// <summary>
        /// 项目
        /// </summary>
        [StringLength(200)]
        public string? Proj { get; set; }
        
        /// <summary>
        /// 执行人
        /// </summary>
        [StringLength(200)]
        public string? DoUser { get; set; }
        
        /// <summary>
        /// 单据状态
        /// </summary>
        public int? BillState { get; set; }
        
        /// <summary>
        /// 价格单据ID（关联Ask_Bill主表）
        /// </summary>
        public int? PriceBillID { get; set; }
        
        /// <summary>
        /// 阀体天数
        /// </summary>
        public int? DayFT { get; set; }
        
        /// <summary>
        /// 执行天数
        /// </summary>
        public int? DayExe { get; set; }
        
        /// <summary>
        /// 附件天数
        /// </summary>
        public int? DayFJ { get; set; }
        
        public int? DayJJ { get; set; }

        public int? DayZP { get; set; }
        
        /// <summary>
        /// 创建人
        /// </summary>
        [StringLength(200)]
        public string? KUser { get; set; }
        
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? KDate { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 采购阀体天数
        /// </summary>
        public int? CGDayFT { get; set; }
        
        /// <summary>
        /// 阀体天数是否禁用
        /// </summary>
        public int? FTDayIsDisabled { get; set; } = 0;
        
        /// <summary>
        /// 采购执行天数
        /// </summary>
        public int? CGDayExe { get; set; }
        
        /// <summary>
        /// 采购附件天数
        /// </summary>
        public int? CGDayFJ { get; set; }
        
        /// <summary>
        /// 附件天数是否禁用
        /// </summary>
        public int? FJDayIsDisabled { get; set; } = 0;
        
        /// <summary>
        /// 是否有前期
        /// </summary>
        public int? IsBefore { get; set; }
        
        /// <summary>
        /// 前期金额
        /// </summary>
        public decimal? BeforeAmount { get; set; }
        
        /// <summary>
        /// 是否无计划
        /// </summary>
        public int? IsNoPlan { get; set; }
        
        /// <summary>
        /// 无计划金额
        /// </summary>
        public decimal? NoPlanAmount { get; set; }
        
        /// <summary>
        /// 前期备注
        /// </summary>
        public string? BeforeMemo { get; set; }
        
        /// <summary>
        /// 生产备注
        /// </summary>
        [StringLength(50)]
        public string? Memo { get; set; }
        
        /// <summary>
        /// 采购备注
        /// </summary>
        [StringLength(100)]
        public string? CGMemo { get; set; }
        
        /// <summary>
        /// 询价备注
        /// </summary>
        [StringLength(50)]
        public string? AskMemo { get; set; }
        
        /// <summary>
        /// 采购日期
        /// </summary>
        public DateTime? CGDate { get; set; }
        
        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? SCDate { get; set; }
        
        /// <summary>
        /// 单据类型
        /// </summary>
        public int? BillType { get; set; } = 0;
        
        /// <summary>
        /// 单据标志
        /// </summary>
        public int? BillFlag { get; set; } = 0;
        
        /// <summary>
        /// 采购员
        /// </summary>
        [StringLength(50)]
        public string? CGUser { get; set; }
        
        /// <summary>
        /// 来源
        /// </summary>
        [StringLength(50)]
        public string? Source { get; set; } = "Mes";
        
        /// <summary>
        /// 客户需求天数
        /// </summary>
        [StringLength(50)]
        public string? CustomNeedDay { get; set; }
        
        /// <summary>
        /// 关闭备注
        /// </summary>
        [StringLength(200)]
        public string? ShutMemo { get; set; }
        
        /// <summary>
        /// BPM单据ID
        /// </summary>
        public string? BpmBillId { get; set; }
        
        /// <summary>
        /// 客户
        /// </summary>
        public string? Customer { get; set; }
        
        /// <summary>
        /// 归属ID
        /// </summary>
        public string? BelongId { get; set; }
        
        /// <summary>
        /// NT3 ID
        /// </summary>
        public string? NT3Id { get; set; }
    }
}
