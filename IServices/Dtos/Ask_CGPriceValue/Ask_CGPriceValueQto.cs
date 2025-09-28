using ZKLT25.API.Helper;

namespace ZKLT25.API.IServices.Dtos
{
    public class Ask_CGPriceValueQto : BasePageParams
    {
        /// <summary>
        /// 型号
        /// </summary>
        public string? Version { get; set; }
        
        /// <summary>
        /// 名称
        /// </summary>
        public string? Name { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        public string? Type { get; set; }
        
        /// <summary>
        /// 客户
        /// </summary>
        public string? Customer { get; set; }
        
        /// <summary>
        /// 有效性筛选：true=仅显示有效，null=显示全部
        /// </summary>
        public bool? IsValid { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string? SuppName { get; set; }
    }
}
