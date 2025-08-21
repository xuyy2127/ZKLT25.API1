using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZKLT25.API.Models
{
    /// <summary>
    /// 报价文件日志表
    /// </summary>
    [Table("Ask_BillFileLog")]
    public class Ask_BillFileLog
    {
        [Key]
        public int ID { get; set; }
        
        /// <summary>
        /// billdetaiID
        /// </summary>
        [Required]
        public int BillDetailID { get; set; }
        
        /// <summary>
        /// 文件名
        /// </summary>
        [Required]
        [StringLength(200)]
        public string FileName { get; set; } = string.Empty;
        
        /// <summary>
        /// 创建日期
        /// </summary>
        [Required]
        public DateTime CreationDate { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 创建人
        /// </summary>
        [Required]
        [StringLength(20)]
        public string CreationPreson { get; set; } = string.Empty;
    }
}
