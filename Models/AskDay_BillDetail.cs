using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZKLT25.API.Models
{
    /// <summary>
    /// 交期明细表（AskDay_BillDetail）
    /// </summary>
    [Table("AskDay_BillDetail")]
    public class AskDay_BillDetail
    {
        /// <summary>
        /// 主键，自增ID。系统内部唯一标识。
        /// </summary>
        [Key]
        public int BillID { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [StringLength(200)]
        public string? Type { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        [StringLength(2000)]
        public string? Version { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(200)]
        public string? Name { get; set; }

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
        /// 阀体材质
        /// </summary>
        [StringLength(200)]
        public string? FT { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public double? Num { get; set; }

        /// <summary>
        /// FG
        /// </summary>
        [StringLength(200)]
        public string? FG { get; set; }

        /// <summary>
        /// 交期状态：-1 已关闭；0 已发起；2 已回复
        /// </summary>
        public int? State { get; set; }
    }
}



















