namespace ZKLT25.API.Models
{
    /// <summary>
    /// 具有ID属性的实体接口
    /// </summary>
    public interface IEntityWithId
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        int ID { get; set; }
    }
}
