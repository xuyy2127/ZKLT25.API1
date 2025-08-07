using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 批量更新系数请求DTO
    /// </summary>
    public class BatchUpdateRatioRequest
    {
        /// <summary>
        /// 要更新的记录ID列表
        /// </summary>
        [Required(ErrorMessage = "请选择要更新的记录")]
        public List<int> Ids { get; set; } = new List<int>();
        
        /// <summary>
        /// 新的系数值
        /// </summary>
        [Required(ErrorMessage = "系数不能为空")]
        [Range(0.01, 9.99, ErrorMessage = "系数值必须大于0且小于10")]
        public double Ratio { get; set; }
    }
} 