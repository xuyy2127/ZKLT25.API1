using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZKLT25.API.Models
{
    /// <summary>
    /// 阀体供货关系表
    /// </summary>
    [Table("Ask_SuppRangeFT")]
    public class Ask_SuppRangeFT
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
        /// 阀体ID
        /// </summary>
        public int? FTID { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        public int? lv { get; set; }

        /// <summary>
        /// 供应商导航属性
        /// </summary>
        [ForeignKey("SuppID")]
        public virtual Ask_Supplier? Supplier { get; set; }

        /// <summary>
        /// 阀体导航属性
        /// </summary>
        [ForeignKey("FTID")]
        public virtual Ask_FTList? FTList { get; set; }
    }
}