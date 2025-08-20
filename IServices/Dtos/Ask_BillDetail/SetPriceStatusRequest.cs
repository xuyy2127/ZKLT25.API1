using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 设置价格状态请求DTO（支持单个和批量）
    /// </summary>
    public class SetPriceStatusRequest
    {
        /// <summary>
        /// 记录ID列表
        /// </summary>
        [Required(ErrorMessage = "记录ID不能为空")]
        public List<int> Ids { get; set; } = new List<int>();
        
        /// <summary>
        /// 操作类型：SETVALID(设置有效)、SETEXPIRED(设置过期)、EXTENDVALID(延长有效期)
        /// </summary>
        [Required(ErrorMessage = "操作类型不能为空")]
        public string Action { get; set; } = "";
        
        /// <summary>
        /// 延长天数（仅在EXTENDVALID时需要）
        /// </summary>
        public int? ExtendDays { get; set; }
        
        /// <summary>
        /// 实体类型：DataFT(阀体) 或 DataFJ(附件)
        /// </summary>
        [Required(ErrorMessage = "实体类型不能为空")]
        public string EntityType { get; set; } = "";
    }
}
