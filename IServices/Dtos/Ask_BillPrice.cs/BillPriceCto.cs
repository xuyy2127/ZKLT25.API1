using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 价格录入请求DTO
    /// </summary>
    public class BillPriceCto
    {
        /// <summary>
        /// 明细ID列表（单个传1个，批量传多个）
        /// </summary>
        [Required(ErrorMessage = "明细ID列表不能为空")]
        public List<int> BillDetailIDs { get; set; } = new List<int>();
        
        /// <summary>
        /// 基础价格
        /// </summary>
        [Required(ErrorMessage = "基础价格不能为空")]
        [Range(0.01, double.MaxValue, ErrorMessage = "输入的价格必须大于0")]
        public double BasicsPrice { get; set; }

        /// <summary>
        /// 附加价格
        /// </summary>
        public double? AddPrice { get; set; }

        /// <summary>
        /// 供应商ID
        /// </summary>
        public int? SuppID { get; set; }

        /// <summary>
        /// 报价回复备注（Ask_BillPrice表）
        /// </summary>
        [StringLength(1000, ErrorMessage = "报价回复备注长度不能超过1000个字符")]
        public string? Remarks { get; set; }
        
        /// <summary>
        /// 报价备注（Ask_BillDetail表）
        /// </summary>
        [StringLength(1000, ErrorMessage = "报价备注长度不能超过1000个字符")]
        public string? CGPriceMemo { get; set; }

        /// <summary>
        /// 采购员备注（Ask_BillDetail表 仅自己可见）
        /// </summary>
        [StringLength(1000, ErrorMessage = "采购员备注长度不能超过1000个字符")]
        public string? CGMemo { get; set; }

        /// <summary>
        /// 价格有效期（天数，可选）
        /// </summary>
        [Range(1, 999, ErrorMessage = "有效期必须在1-999天之间")]
        public int? ValidityDays { get; set; } = 1;
        
        /// <summary>
        /// 是否绑定项目（0=不绑定，1=绑定，可选）
        /// </summary>
        [Range(0, 1, ErrorMessage = "绑定项目值必须为0或1")]
        public int? IsPreProBind { get; set; } = 0;
        
        /// <summary>
        /// 上传的报价文件
        /// </summary>
        public IFormFile? QuoteFile { get; set; }
    }
}
