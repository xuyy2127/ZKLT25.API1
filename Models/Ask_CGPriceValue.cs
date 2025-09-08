using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.Models
{
    public class Ask_CGPriceValue
    {
        [Key]
        public int Id { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        public string? Type { get; set; }
        
        /// <summary>
        /// 型号
        /// </summary>
        public string? Version { get; set; }
        
        /// <summary>
        /// 名称
        /// </summary>
        public string? Name { get; set; }
        
        /// <summary>
        /// 口径
        /// </summary>
        public string? DN { get; set; }
        
        /// <summary>
        /// 压力
        /// </summary>
        public string? PN { get; set; }
        
        /// <summary>
        /// LJ
        /// </summary>
        public string? LJ { get; set; }
        
        /// <summary>
        /// FG
        /// </summary>
        public string? FG { get; set; }
        
        /// <summary>
        /// FT
        /// </summary>
        public string? FT { get; set; }
        
        /// <summary>
        /// FNJ
        /// </summary>
        public string? FNJ { get; set; }
        
        /// <summary>
        /// 数量
        /// </summary>
        public double? Num { get; set; }
        
        /// <summary>
        /// ordMed
        /// </summary>
        public string? ordMed { get; set; }
        
        /// <summary>
        /// OrdKV
        /// </summary>
        public string? OrdKV { get; set; }
        
        /// <summary>
        /// ordFW
        /// </summary>
        public string? ordFW { get; set; }
        
        /// <summary>
        /// ordLeak
        /// </summary>
        public string? ordLeak { get; set; }
        
        /// <summary>
        /// 气源压力
        /// </summary>
        public string? ordQY { get; set; }
        
        /// <summary>
        /// TL
        /// </summary>
        public string? TL { get; set; }
        
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? ExpireTime { get; set; }
        
        /// <summary>
        /// 基础价格
        /// </summary>
        public double? Price { get; set; }
        
        /// <summary>
        /// 价格备注
        /// </summary>
        public string? PriceMemo { get; set; }
        
        /// <summary>
        /// 供应商ID
        /// </summary>
        public int? SuppId { get; set; }
        
        /// <summary>
        /// 价格ID
        /// </summary>
        public int? PriceId { get; set; }
        
        /// <summary>
        /// 加价
        /// </summary>
        public double? AddPrice { get; set; } = 0;
        
        /// <summary>
        /// 客户
        /// </summary>
        public string? Customer { get; set; }
    }
}
