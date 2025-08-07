using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 询价供应商创建请求DTO
    /// </summary>
    public class Ask_SupplierCto
    {
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
    }
}