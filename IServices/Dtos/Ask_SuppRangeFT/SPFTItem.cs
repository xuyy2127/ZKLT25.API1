namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 阀体配置项（用于批量更新接口的数组元素）
    /// </summary>
    public class SPFTItem
    {
        /// <summary>
        /// 阀体ID
        /// </summary>
        public int FTID { get; set; }

        /// <summary>
        /// 供应等级：3推荐，2一般，1备用，可为空
        /// </summary>
        public int? Lv { get; set; } = null;
    }
}








