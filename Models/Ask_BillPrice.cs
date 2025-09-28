using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.Models
{
    public class Ask_BillPrice
    {
        [Key]
        public int ID { get; set; }
        
        /// <summary>
        /// 询价详情单ID
        /// </summary>
        public int? BillDetailID { get; set; }
        
        /// <summary>
        /// 开票日期
        /// </summary>
        public DateTime? KDate { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 开票用户
        /// </summary>
        public string? KUser { get; set; }
        
        /// <summary>
        /// 价格
        /// </summary>
        public double? Price { get; set; }
        
        /// <summary>
        /// 数量
        /// </summary>
        public double? Num { get; set; }
        
        /// <summary>
        /// 供应商ID
        /// </summary>
        public int? SuppID { get; set; }
        
        /// <summary>
        /// 价格比例
        /// </summary>
        public double? PriceRatio { get; set; }
        
        /// <summary>
        /// 基础价格
        /// </summary>
        public double? BasicsPrice { get; set; } = 0;
        
        /// <summary>
        /// 附加价格
        /// </summary>
        public double? AddPrice { get; set; } = 0;
        
        /// <summary>
        /// 备注
        /// </summary>
        public string? Remarks { get; set; }
    }
}


