namespace ZKLT25.API.Helper
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public class BizException(string message) : Exception(message)
    {
        /// <summary>
        /// 
        /// </summary>
        public override string Message
        {
            get
            {
                return message;
            }
        }
    }
}
