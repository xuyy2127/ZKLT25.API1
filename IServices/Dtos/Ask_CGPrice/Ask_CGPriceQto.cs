using ZKLT25.API.Helper;

namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// Ask_CGPrice 查询条件
    /// </summary>
    public class Ask_CGPriceQto : BasePageParams
    {
        /// <summary>
        /// 名称（Name）
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 型号（Version）
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// 项目名称（ProjName，通过关联 Ask_Bill 获取时可作为筛选关键字）
        /// </summary>
        public string? ProjName { get; set; }

        /// <summary>
        /// 类型ID列表（来自 FJList 的 ID 集合）
        /// </summary>
        public List<int> Types { get; set; } = new();
    }
}