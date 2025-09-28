using ZKLT25.API.Helper;

namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 询价供应商查询条件DTO
    /// </summary>
    public class Ask_SupplierQto : BasePageParams
    {
        /// <summary>
        /// 关键字搜索
        /// </summary>
        public string? Keyword { get; set; }
    }
}