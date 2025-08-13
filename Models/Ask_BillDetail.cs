using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.Models
{
    public class Ask_BillDetail
    {
        [Key]
        public int ID { get; set; }
        
        /// <summary>
        /// 关联ID
        /// </summary>
        public int? BillID { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        public string? Type { get; set; }
        
        /// <summary>
        /// 版本
        /// </summary>
        public string? Version { get; set; }
        
        /// <summary>
        /// 名称
        /// </summary>
        public string? Name { get; set; }
        
        /// <summary>
        /// DN
        /// </summary>
        public string? DN { get; set; }
        
        /// <summary>
        /// PN
        /// </summary>
        public string? PN { get; set; }
        
        /// <summary>
        /// 连接
        /// </summary>
        public string? LJ { get; set; }
        
        /// <summary>
        /// 阀盖
        /// </summary>
        public string? FG { get; set; }
        
        /// <summary>
        /// 阀体
        /// </summary>
        public string? FT { get; set; }
        
        /// <summary>
        /// 阀内件
        /// </summary>
        public string? FNJ { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string? Memo { get; set; }
        
        /// <summary>
        /// 数量
        /// </summary>
        public float? Num { get; set; }
        
        /// <summary>
        /// 状态
        /// </summary>
        public int? State { get; set; }
        
        /// <summary>
        /// 采购价格备注
        /// </summary>
        public string? CGPriceMemo { get; set; }
        
        /// <summary>
        /// 采购备注
        /// </summary>
        public string? CGMemo { get; set; }
        
        /// <summary>
        /// 订单介质
        /// </summary>
        public string? ordMed { get; set; }
        
        /// <summary>
        /// 订单KV值
        /// </summary>
        public string? OrdKV { get; set; }
        
        /// <summary>
        /// 订单法兰
        /// </summary>
        public string? ordFW { get; set; }
        
        /// <summary>
        /// 订单泄漏
        /// </summary>
        public string? ordLeak { get; set; }
        
        /// <summary>
        /// 订单驱动
        /// </summary>
        public string? ordQY { get; set; }
        
        /// <summary>
        /// 填料
        /// </summary>
        public string? TL { get; set; }
        
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? ExpireTime { get; set; }
        
        /// <summary>
        /// 超时
        /// </summary>
        public int? Timeout { get; set; } = -1;
    }
}
