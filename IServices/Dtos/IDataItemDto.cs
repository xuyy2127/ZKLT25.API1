namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 阀体附件价格查询通用接口
    /// </summary>
    public interface IDataItemDto
    {
        /// <summary>
        /// 是否绑定项目
        /// </summary>
        int IsPreProBind { get; set; }
        
        /// <summary>
        /// 超时状态
        /// </summary>
        int? Timeout { get; set; }
        
        /// <summary>
        /// 是否过期（1=有效，0=已过期）
        /// </summary>
        int IsInvalid { get; set; }
        
        /// <summary>
        /// 文档名称
        /// </summary>
        string? DocName { get; set; }
        
        /// <summary>
        /// 文件名
        /// </summary>
        string? FileName { get; set; }
        
        /// <summary>
        /// 项目绑定状态显示文本
        /// </summary>
        string IsPreProBindText { get; set; }
        
        /// <summary>
        /// 价格状态显示文本
        /// </summary>
        string PriceStatusText { get; set; }
        
        /// <summary>
        /// 可执行的价格状态操作
        /// </summary>
        string AvailableActions { get; set; }
        
        /// <summary>
        /// 文档状态显示文本
        /// </summary>
        string DocNameStatus { get; set; }
        
        /// <summary>
        /// 文件状态显示文本
        /// </summary>
        string FileNameStatus { get; set; }
    }
}
