using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.Models
{
    public class Ask_BillLog
    {
        [Key]
        public int ID { get; set; }
        
        /// <summary>
        /// 账单详情ID
        /// </summary>
        public int? BillDetailID { get; set; }
        
        /// <summary>
        /// 状态
        /// </summary>
        public int? State { get; set; }
        
        /// <summary>
        /// 记录日期
        /// </summary>
        public DateTime? KDate { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 操作用户
        /// </summary>
        public string? KUser { get; set; }
    }
}
