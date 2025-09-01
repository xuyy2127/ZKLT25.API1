namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 通用的查询输入接口 (Qto)
    /// </summary>
    public interface IPriceDataQto
    {
        int PageNumber { get; set; }
        int PageSize { get; set; }
        bool? IsExpired { get; set; }
        DateTime? StartDate { get; set; }
        DateTime? EndDate { get; set; }
        string? AskProjName { get; set; }
        string? SuppName { get; set; }
        int? SuppID { get; set; }
        int? BillID { get; set; }
    }

    public class BaseDto
    {
    }
}
