namespace ZKLT25.API.Models.Interfaces
{
    /// <summary>
    /// 超时绑定实体共享接口
    /// 定义具有 Timeout、IsPreProBind、DoUser 和 DoDate 属性的实体契约
    /// </summary>
    public interface ITimeoutBindable
    {
        /// <summary>
        /// 超时状态
        /// </summary>
        int? Timeout { get; set; }
        
        /// <summary>
        /// 是否预绑定
        /// </summary>
        int IsPreProBind { get; set; }
        
        /// <summary>
        /// 执行用户
        /// </summary>
        string? DoUser { get; set; }
        
        /// <summary>
        /// 执行日期
        /// </summary>
        DateTime? DoDate { get; set; }
    }
}
