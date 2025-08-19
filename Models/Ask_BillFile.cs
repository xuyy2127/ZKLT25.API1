using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZKLT25.API.Models
{
    /// <summary>
    /// 询价账单文件表
    /// </summary>
    [Table("Ask_BillFile")]
    public class Ask_BillFile
    {
        [Key]
        public int ID { get; set; }
        
        /// <summary>
        /// 账单详情ID
        /// </summary>
        public int BillDetailID { get; set; }
        
        /// <summary>
        /// 文件名
        /// </summary>
        [StringLength(200)]
        public string? FileName { get; set; }
        
        /// <summary>
        /// 状态
        /// </summary>
        public int State { get; set; } = 1;
    }
}



