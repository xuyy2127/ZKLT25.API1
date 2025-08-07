using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZKLT25.API.Models
{
    /// <summary>
    /// 询价供应商信息表
    /// </summary>
    [Table("Ask_Supplier")]
    public class Ask_Supplier
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        [Required(ErrorMessage = "供应商名称不能为空")]
        [StringLength(200, ErrorMessage = "供应商名称长度不能超过200个字符")]
        public string SuppName { get; set; } = "";

        /// <summary>
        /// 供应商类型 (0=不合格、1=合格、2=临时、3=初选)
        /// </summary>
        public int SupplierClass { get; set; } = 0;

        /// <summary>
        /// 创建人
        /// </summary>
        [StringLength(50)]
        public string KUser { get; set; } = "";

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? KDate { get; set; }
    }
}