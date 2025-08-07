namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 供应商阀体配置页面DTO
    /// </summary>
    public class SupplierFTConfigPageDto
    {
        /// <summary>
        /// 阀体ID
        /// </summary>
        public int FTID { get; set; }

        /// <summary>
        /// 阀体名称
        /// </summary>
        public string FTName { get; set; } = "";

        /// <summary>
        /// 阀体版本
        /// </summary>
        public string FTVersion { get; set; } = "";

        /// <summary>
        /// 是否供应（用于前端勾选状态）
        /// </summary>
        public bool IsSupplied { get; set; } = false;

        /// <summary>
        /// 等级（1-3，可选）
        /// </summary>
        public int? Lv { get; set; } = null;
    }
} 