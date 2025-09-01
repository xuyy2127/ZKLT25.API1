namespace ZKLT25.API.Models
{
    /// <summary>
    /// 具有比例属性的实体接口
    /// </summary>
    public interface IEntityWithRatio
    {
        /// <summary>
        /// 价格系数
        /// </summary>
        double? ratio { get; set; }
    }
}


