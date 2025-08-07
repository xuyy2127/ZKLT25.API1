using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 阀体型号更新请求DTO
    /// </summary>
    public class Ask_FTListUto
    {
        /// <summary>
        /// 阀体型号
        /// </summary>
        [Required(ErrorMessage = "阀体型号不能为空")]
        [StringLength(100, ErrorMessage = "阀体型号长度不能超过100个字符")]
        public string FTVersion { get; set; } = "";
        
        /// <summary>
        /// 阀体名称
        /// </summary>
        [Required(ErrorMessage = "阀体名称不能为空")]
        [StringLength(200, ErrorMessage = "阀体名称长度不能超过200个字符")]
        public string FTName { get; set; } = "";
        
        /// <summary>
        /// 是否外购 (1=外购, 0=自制)
        /// </summary>
        public int? isWG { get; set; } = 1;
        
        /// <summary>
        /// 比例系数
        /// </summary>
        [Range(0.01, 9.99, ErrorMessage = "系数值必须大于0且小于10")]
        public double? ratio { get; set; } = 1.0;
        
        /// <summary>
        /// 是否询价 (1=是, 0=否)
        /// </summary>
        public int? isAsk { get; set; } = 0;
    }
}