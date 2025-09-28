using ZKLT25.API.Helper;
using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.IServices.Dtos
{
    public class AskDay_BillDto
    {
        public int BillID { get; set; }
        public string? ProjName { get; set; }
        public string? Customer { get; set; }
        public string? DocName { get; set; }
        public string? DoUser { get; set; }
        public string? KUser { get; set; }
        public DateTime? KDate { get; set; }
        public DateTime? SCDate { get; set; }
        public DateTime? CGDate { get; set; }
        public int? BillState { get; set; }
        public string StateText { get; set; } = string.Empty; // -1已关闭, 0发起, 1采购回复, 2已完成
        public string? TodoType { get; set; } // 待办类型文本，“生产未回复”“生产驳回”“采购未回复”“已完成”“关闭”
        public int? DayFT { get; set; }
        public int? DayFJ { get; set; }
        public int? DayExe { get; set; }
        public int? CGDayFT { get; set; }
        public int? CGDayFJ { get; set; }
        public int? CGDayExe { get; set; }
        public int? DayJJ { get; set; }
        public int? DayZP { get; set; }
        public string? Memo { get; set; } // 生产备注或驳回原因
        public string? CGMemo { get; set; } // 采购备注
        public string? ShutMemo { get; set; } // 关闭备注
        public int? AskBillID { get; set; }// 询价单据号（来自 Ask_Bill.BillID）
        public bool HasHistory { get; set; } // 是否存在历史
        public string? ProjUser { get; set; }
        public string? CGUser { get; set; }
    }

    public class AskDay_BillQto : BasePageParams
    {
        public int? BillID { get; set; }
        public string? ProjName { get; set; }
        public string? Customer { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Type { get; set; } // 类型筛选 product生产/purchase采购/completed已完成/closed关闭
        public int? BillState { get; set; }
    }

    public class AskDay_BillHistoryQto
    {
        [Required]
        public int BillID { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

}


