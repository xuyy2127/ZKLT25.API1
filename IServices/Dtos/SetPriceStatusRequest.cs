using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 设置价格状态请求DTO
    /// </summary>
    public class SetPriceStatusRequest
    {
        /// <summary>
        /// 记录ID
        /// </summary>
        [Required(ErrorMessage = "记录ID不能为空")]
        public int Id { get; set; }
        
        /// <summary>
        /// 操作类型：SetValid(设置有效)、SetExpired(设置过期)、ExtendValid(延长有效期)
        /// </summary>
        [Required(ErrorMessage = "操作类型不能为空")]
        public string Action { get; set; } = "";
        
        /// <summary>
        /// 延长天数（仅在ExtendValid时需要）
        /// </summary>
        public int? ExtendDays { get; set; }
    }
}
