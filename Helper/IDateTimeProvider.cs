namespace ZKLT25.API.Helper
{
    /// <summary>
    /// 时间提供者接口，用于统一时间处理
    /// </summary>
    public interface IDateTimeProvider
    {
        /// <summary>
        /// 获取当前本地时间
        /// </summary>
        DateTime Now { get; }
        
        /// <summary>
        /// 获取当前UTC时间
        /// </summary>
        DateTime UtcNow { get; }
        
        /// <summary>
        /// 获取当前日期
        /// </summary>
        DateTime Today { get; }
    }
}


