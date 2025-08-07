namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 附件查询响应DTO
    /// </summary>
    public class Ask_FJListDto
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int ID { get; set; }
        
        /// <summary>
        /// 附件类型
        /// </summary>
        public string? FJType { get; set; }
        
        /// <summary>
        /// 比例系数
        /// </summary>
        public double? ratio { get; set; }
    }
}