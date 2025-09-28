namespace ZKLT25.API.IServices.Dtos
{
    public class Ask_CGPriceValueDto
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 型号
        /// </summary>
        public string? Version { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        public string? Type { get; set; }
        
        /// <summary>
        /// 口径
        /// </summary>
        public string? DN { get; set; }
        
        /// <summary>
        /// 压力
        /// </summary>
        public string? PN { get; set; }
        
        /// <summary>
        /// 气源压力
        /// </summary>
        public string? ordQY { get; set; }
        
        /// <summary>
        /// 基础价格
        /// </summary>
        public double? Price { get; set; }
        
        /// <summary>
        /// 加价
        /// </summary>
        public double? AddPrice { get; set; }
        
        /// <summary>
        /// 截止日期
        /// </summary>
        public DateTime? ExpireTime { get; set; }

        public int? EffectDay { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string? PriceMemo { get; set; }
        
        /// <summary>
        /// 客户
        /// </summary>
        public string? Customer { get; set; }
        
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }

        public int? SuppID { get; set; }

        public string SuppName { get; set; }
    }
}
