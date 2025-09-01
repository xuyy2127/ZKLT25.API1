namespace ZKLT25.API.Models
{
    /// <summary>
    /// 附件数据实体接口（Ask_DataFJ 和 Ask_DataFJOut 的公共属性）
    /// </summary>
    public interface IDataFJEntity : IEntityWithId, ITimeoutBindable
    {
        /// <summary>
        /// 来源
        /// </summary>
        string? Source { get; set; }
        
        /// <summary>
        /// 询价日期
        /// </summary>
        DateTime? AskDate { get; set; }
        
        /// <summary>
        /// 询价项目名称
        /// </summary>
        string? AskProjName { get; set; }
        
        /// <summary>
        /// 附件类型
        /// </summary>
        string? FJType { get; set; }
        
        /// <summary>
        /// 附件型号
        /// </summary>
        string? FJVersion { get; set; }
        
        /// <summary>
        /// 单位
        /// </summary>
        string? Unit { get; set; }
        
        /// <summary>
        /// 数量
        /// </summary>
        int? Num { get; set; }
        
        /// <summary>
        /// 价格
        /// </summary>
        double? Price { get; set; }
        
        /// <summary>
        /// 供应商
        /// </summary>
        int? Supplier { get; set; }
        
        /// <summary>
        /// 项目天数
        /// </summary>
        string? ProjDay { get; set; }
        
        /// <summary>
        /// 天数1
        /// </summary>
        string? Day1 { get; set; }
        
        /// <summary>
        /// 天数2
        /// </summary>
        string? Day2 { get; set; }
        
        /// <summary>
        /// 天数3
        /// </summary>
        string? Day3 { get; set; }
        
        /// <summary>
        /// 备注1
        /// </summary>
        string? Memo1 { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        string? Memo { get; set; }
        
        /// <summary>
        /// 价格比例
        /// </summary>
        double? PriceRatio { get; set; }
        
        /// <summary>
        /// 单据明细ID
        /// </summary>
        int? BillDetailID { get; set; }
        
        /// <summary>
        /// 公称通径
        /// </summary>
        string? DN { get; set; }
        
        /// <summary>
        /// 公称压力
        /// </summary>
        string? PN { get; set; }
        
        /// <summary>
        /// 连接方式
        /// </summary>
        string? OrdLJ { get; set; }
    }
}
