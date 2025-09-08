using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.IServices.Dtos
{
    public class Ask_CGPriceValueCto
    {
        /// <summary>
        /// 供应商ID
        /// </summary>
        [Required(ErrorMessage = "请选择供应商")]
        public int? SuppId { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        [Required(ErrorMessage = "类型不能为空")]
        [StringLength(500, ErrorMessage = "类型长度不能超过500个字符")]
        public string Type { get; set; } = "";
        
        /// <summary>
        /// 型号
        /// </summary>
        [Required(ErrorMessage = "型号不能为空")]
        [StringLength(2000, ErrorMessage = "型号长度不能超过2000个字符")]
        public string Version { get; set; } = "";
        
        /// <summary>
        /// 基础价格
        /// </summary>
        [Required(ErrorMessage = "基础价格不能为空")]
        [Range(0.01, double.MaxValue, ErrorMessage = "基础价格必须大于0")]
        public double Price { get; set; }
        
        /// <summary>
        /// 加价金额
        /// </summary>
        public double? AddPrice { get; set; } = 0;
        
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
        /// 有效期（不填默认3650天）
        /// </summary>
        public DateTime? ExpireTime { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string? PriceMemo { get; set; }
        
        /// <summary>
        /// 客户关键字（客户制询价必填）
        /// </summary>
        public string? Customer { get; set; }
    }
}
