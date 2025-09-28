using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 供应商更新请求DTO
    /// </summary>
    public class Ask_SupplierUto
    {
        /// <summary>
        /// 供应商名称（可选，若不修改名称可不传）
        /// </summary>
        [StringLength(200, ErrorMessage = "供应商名称长度不能超过200个字符")]
        public string? SuppName { get; set; }

        /// <summary>
        /// 供应商类型 (0=不合格、1=合格、2=临时、3=初选)，未传则不变
        /// </summary>
        public int? SupplierClass { get; set; }
    }
}