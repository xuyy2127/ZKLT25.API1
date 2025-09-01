using ZKLT25.API.Helper;

namespace ZKLT25.API.IServices.Dtos
{
    public class Ask_DataFTQto : BasePageParams, IPriceDataQto
    {
        /// <summary>
        /// 交期编号
        /// </summary>
        public int? BillID { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string? AskProjName { get; set; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string? SuppName { get; set; }
        /// <summary>
        /// 供应商ID
        /// </summary>
        public int? SuppID { get; set; }
        /// <summary>
        /// 阀体型号
        /// </summary>
        public string? OrdVersion { get; set; }
        /// <summary>
        /// 公称通径
        /// </summary>
        public string? OrdDN { get; set; }
        /// <summary>
        /// 公称压力
        /// </summary>
        public string? OrdPN { get; set; }
        /// <summary>
        /// 阀体材质
        /// </summary>
        public string? OrdFT { get; set; }
        
        /// <summary>
        /// 阀体ID
        /// </summary>
        public int? FTTypeId { get; set; }
        /// <summary>
        /// 查询开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 查询结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 价格状态过滤（null=全部，true=已过期，false=未过期）
        /// </summary>
        public bool? IsExpired { get; set; }
    }
}