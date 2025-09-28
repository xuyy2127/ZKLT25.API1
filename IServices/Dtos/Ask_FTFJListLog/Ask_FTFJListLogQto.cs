using ZKLT25.API.Helper;

namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 阀体 / 附件日志查询请求DTO
    /// </summary>
    public class Ask_FTFJListLogQto : BasePageParams
    {
        /// <summary>
        /// 关联主表ID
        /// </summary>
        public int? MainID { get; set; }
        
        /// <summary>
        /// 数据类型 ：FT 或 FJ
        /// </summary>
        public string? DataType { get; set; }
    }
} 