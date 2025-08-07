namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 批量更新供应商阀体配置请求DTO
    /// </summary>
    public class BatchUpdateSPFTCto
    {
        /// <summary>
        /// 选中的阀体配置列表
        /// </summary>
        public List<SPFTItem> SuppliedFTItems { get; set; } = new();
    }

    /// <summary>
    /// 供应商阀体配置项
    /// </summary>
    public class SPFTItem
    {
        /// <summary>
        /// 阀体ID
        /// </summary>
        public int FTID { get; set; }

        /// <summary>
        /// 等级:3推荐，2一般，1备用，可以为null
        /// </summary>
        public int? Lv { get; set; } = null;
    }
}
