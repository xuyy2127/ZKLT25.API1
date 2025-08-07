using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 询价供应商数据传输对象
    /// </summary>
    public class Ask_SupplierDto
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int ID { get; set; }
       
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SuppName { get; set; } = "";

        /// <summary>
        /// 供应商类型 (0=不合格、1=合格、2=临时、3=初选)
        /// </summary>
        public int SupplierClass { get; set; } = 0;

        /// <summary>
        /// 创建人
        /// </summary>
        public string KUser { get; set; } = "";

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? KDate { get; set; }
    }
}