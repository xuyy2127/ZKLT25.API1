namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 供应商阀体配置页面DTO
    /// </summary>
    public class SPFTPageDto
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
        /// 等级:3推荐，2一般，1备用，可以为null
        /// </summary>
        public int? Lv { get; set; } = null;
    }
}
