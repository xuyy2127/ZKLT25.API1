using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZKLT25.API.Models
{
    /// <summary>
    /// 附件供货关系表
    /// </summary>
    [Table("Ask_SuppRangeFJ")]
    public class Ask_SuppRangeFJ
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// 供应商ID
        /// </summary>
        public int? SuppID { get; set; }

        /// <summary>
        /// 附件类型
        /// </summary>
        [StringLength(50)]
        public string? FJType { get; set; }

        /// <summary>
        /// 供应商导航属性
        /// </summary>
        [ForeignKey("SuppID")]
        public virtual Ask_Supplier? Supplier { get; set; }
    }
}