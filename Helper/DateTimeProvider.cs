namespace ZKLT25.API.Helper
{
    /// <summary>
    /// 时间提供者实现，提供当前时间
    /// </summary>
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime Today => DateTime.Today;
    }
}

