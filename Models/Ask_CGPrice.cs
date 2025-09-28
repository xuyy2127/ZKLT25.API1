using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZKLT25.API.Models
{
    [Table("Ask_CGPrice")]
    public class Ask_CGPrice
    {
        /// <summary>
        /// 关联billid
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [StringLength(500)]
        public string? Type { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        [StringLength(2000)]
        public string? Version { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(500)]
        public string? Name { get; set; }

        /// <summary>
        /// 通径
        /// </summary>
        [StringLength(500)]
        public string? DN { get; set; }

        /// <summary>
        /// 压力
        /// </summary>
        [StringLength(500)]
        public string? PN { get; set; }

        /// <summary>
        /// 连接标准
        /// </summary>
        [StringLength(500)]
        public string? LJ { get; set; }

        /// <summary>
        /// 上法兰连接方式
        /// </summary>
        [StringLength(500)]
        public string? FG { get; set; }

        /// <summary>
        /// 阀体材质
        /// </summary>
        [StringLength(500)]
        public string? FT { get; set; }

        /// <summary>
        /// 阀内件材质
        /// </summary>
        [StringLength(500)]
        public string? FNJ { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public double? Num { get; set; }

        /// <summary>
        /// 介质/温度
        /// </summary>
        [StringLength(500)]
        public string? ordMed { get; set; }

        /// <summary>
        /// KV值
        /// </summary>
        [StringLength(500)]
        public string? OrdKV { get; set; }

        /// <summary>
        /// 法兰
        /// </summary>
        [StringLength(500)]
        public string? ordFW { get; set; }

        /// <summary>
        /// 泄漏等级
        /// </summary>
        [StringLength(500)]
        public string? ordLeak { get; set; }

        /// <summary>
        /// 气源压力
        /// </summary>
        [StringLength(500)]
        public string? ordQY { get; set; }

        /// <summary>
        /// 填料
        /// </summary>
        [StringLength(500)]
        public string? TL { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? ExpireTime { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? State { get; set; }

        /// <summary>
        /// 价格备注
        /// </summary>
        [StringLength(1000)]
        public string? PriceMemo { get; set; }

        /// <summary>
        /// 加价
        /// </summary>
        public double? AddPrice { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime KDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建用户
        /// </summary>
        [StringLength(50)]
        public string? KUser { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime? UDate { get; set; }

        /// <summary>
        /// 更新用户
        /// </summary>
        [StringLength(50)]
        public string? UUser { get; set; }

        /// <summary>
        /// 供应商ID
        /// </summary>
        public int? SuppId { get; set; }
    }
}
