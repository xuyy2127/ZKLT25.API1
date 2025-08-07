namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 批量更新供应商阀体配置请求DTO
    /// </summary>
    public class BatchUpdateSupplierFTConfigCto
    {
        /// <summary>
        /// 选中的阀体配置列表
        /// </summary>
        public List<SupplierFTConfigItem> SuppliedFTItems { get; set; } = new();
    }

    /// <summary>
    /// 供应商阀体配置项
    /// </summary>
    public class SupplierFTConfigItem
    {
        /// <summary>
        /// 阀体ID
        /// </summary>
        public int FTID { get; set; }

        /// <summary>
        /// 等级（1-3，可选）
        /// </summary>
        public int? Lv { get; set; } = null;
    }
} 