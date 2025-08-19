namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 供应商附件配置页面DTO
    /// </summary>
    public class SPFJPageDto
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int ID { get; set; }
        
        /// <summary>
        /// 附件类型
        /// </summary>
        public string FJType { get; set; } = "";

        /// <summary>
        /// 是否供应
        /// </summary>
        public bool IsSupplied { get; set; } = false;
    }
}
