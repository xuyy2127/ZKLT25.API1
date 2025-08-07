using ZKLT25.API.Helper;

namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 附件查询条件DTO
    /// </summary>
    public class Ask_FJListQto : BasePageParams
    {
        /// <summary>
        /// 附件类型关键字搜索
        /// </summary>
        public string? Keyword { get; set; }
    }
}