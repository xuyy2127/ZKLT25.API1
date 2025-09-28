namespace ZKLT25.API.Models.Interfaces
{
    /// <summary>
    /// 阀体数据实体共享接口
    /// 定义 Ask_DataFT 和 Ask_DataFTOut 共有的属性契约
    /// </summary>
    public interface IDataFTEntity : IEntityWithId
    {
        // 基础属性
        string? Source { get; set; }
        DateTime? AskDate { get; set; }
        string? AskProjName { get; set; }
        string? OrdNum { get; set; }
        string? OrdMed { get; set; }
        string? OrdName { get; set; }
        string? OrdVersion { get; set; }
        string? OrdDN { get; set; }
        string? OrdPN { get; set; }
        string? OrdLJ { get; set; }
        string? OrdFG { get; set; }
        string? OrdFT { get; set; }
        string? OrdFNJ { get; set; }
        string? OrdTL { get; set; }
        string? OrdKV { get; set; }
        string? OrdFlow { get; set; }
        string? OrdLeak { get; set; }
        string? OrdQYDY { get; set; }
        int? Num { get; set; }
        string? OrdUnit { get; set; }
        int IsPreProBind { get; set; }
        int? Timeout { get; set; }
        string? ProjDay { get; set; }
        string? Day1 { get; set; }
        string? Day2 { get; set; }
        string? Memo1 { get; set; }
        double? Price { get; set; }
        // 关联属性
        int? Supplier { get; set; }
        int? BillDetailID { get; set; }
    }
}
