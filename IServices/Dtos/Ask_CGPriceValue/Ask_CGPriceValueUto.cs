using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.IServices.Dtos
{
    public class Ask_CGPriceValueUto
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int Id { get; set; }
        
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
        /// 口径（仅类型为"配对法兰及螺栓螺母"时可填写）
        /// </summary>
        public string? DN { get; set; }
        
        /// <summary>
        /// 压力（仅类型为"配对法兰及螺栓螺母"时可填写）
        /// </summary>
        public string? PN { get; set; }
        
        /// <summary>
        /// 气源压力（仅类型为"执行机构"时可填写）
        /// </summary>
        public string? ordQY { get; set; }
        
        /// <summary>
        /// 有效期
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

        /// <summary>
        /// 供应商ID
        /// </summary>
        public int? SuppId { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string? SuppName { get; set; }
    }
}
