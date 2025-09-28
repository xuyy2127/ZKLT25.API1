using ZKLT25.API.Helper;
using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.IServices.Dtos
{
    public class DayFTDto : BaseDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Version { get; set; }
        public string? DN { get; set; }
        public string? PN { get; set; }
        public string? FT { get; set; }
        public string? Sup { get; set; }
        public string? N1 { get; set; }
        public string? N2 { get; set; }
        public string? Day { get; set; }
        public string? Type { get; set; }
        public DateTime? DoDate { get; set; }
        public string? DoUser { get; set; }
    }

    public class DayFTQto : BasePageParams
    {
        public string? Version { get; set; }
        public string? DN { get; set; }
        public string? PN { get; set; }
        public string? FT { get; set; }
        public string? Type { get; set; }
        public string? IsWG { get; set; }
    }

    public class DayFJDto : BaseDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Version { get; set; }
        public string? Sup { get; set; }
        public string? N1 { get; set; }
        public string? N2 { get; set; }
        public string? Day { get; set; }
        public string? Type { get; set; }
        public DateTime? DoDate { get; set; }
        public string? DoUser { get; set; }
    }

    public class DayFJQto : BasePageParams
    {
        public string? Version { get; set; }
        public string? Type { get; set; }
    }
	public class AskDay_BillDetailDto
	{
		public int ID { get; set; }
		public int BillID { get; set; }
		public string? Type { get; set; }
		public string? GroupType { get; set; } // 阀体/执行机构/附件
		public string? Name { get; set; }
		public string? Version { get; set; }
		public string? DN { get; set; }
		public string? PN { get; set; }
		public string? FT { get; set; }
		public double? Num { get; set; }
		public string? Day { get; set; }
		public string? IsWGText { get; set; }
        public int? DayJJ { get; set; }
        public int? DayZP { get; set; }
        [StringLength(50)]
        public string? CGUser { get; set; }
        public DateTime? CGDate { get; set; }
        public int? CGDayFT { get; set; }
        public int? FTDayIsDisabled { get; set; } = 0;
        public int? CGDayExe { get; set; }
        public int? CGDayFJ { get; set; }
        public int? FJDayIsDisabled { get; set; } = 0;
        public int? IsBefore { get; set; }
        public decimal? BeforeAmount { get; set; }
        public int? IsNoPlan { get; set; }
        public decimal? NoPlanAmount { get; set; }
        public string? BeforeMemo { get; set; }
        public string? Memo { get; set; } // 生产备注
        public string? ShutMemo { get; set; }
        public string? CGMemo { get; set; }
    }

	public class AskDay_BillDetailQto : BasePageParams
	{
		public int BillID { get; set; }
		public string? Type { get; set; }
		public string? Version { get; set; }
        public int? FTDayIsDisabled { get; set; } = 0;
        public int? FJDayIsDisabled { get; set; } = 0;
	}

    public class AskDay_BillDetailCto : BasePageParams
    {
        public bool? Submit { get; set; }
        [StringLength(50)]
        public string? CGUser { get; set; }
        public DateTime? CGDate { get; set; }
        public int? CGDayFT { get; set; }
        public int? FTDayIsDisabled { get; set; } = 0;
        public int? CGDayExe { get; set; }
        public int? CGDayFJ { get; set; }
        public int? FJDayIsDisabled { get; set; } = 0;
        public int? IsBefore { get; set; }
        public decimal? BeforeAmount { get; set; }
        public int? IsNoPlan { get; set; }
        public decimal? NoPlanAmount { get; set; }
        public string? BeforeMemo { get; set; }
        [StringLength(50)]
        public string? Memo { get; set; }
        [StringLength(100)]
        public string? CGMemo { get; set; }
        public int? DayJJ { get; set; }
        public int? DayZP { get; set; }
        public DateTime? SCDate { get; set; }
        public string? ShutMemo { get; set; }
        public int? BillState { get; set; }
    }

	public class DaySaveCto
	{
		public string? Type { get; set; }
		public string? Version { get; set; }
		public string? DN { get; set; }
		public string? PN { get; set; }
		public string? FT { get; set; }
		public string? N1 { get; set; }
		public string? N2 { get; set; }
		public string? Day { get; set; }
		// 决定是否写回 Day 表：true=写回，false=仅本次计算生效
		public bool PersistToDayTable { get; set; } = false;
	}

    public class SuggestDaysResult
    {
        public int FtExternalMax { get; set; }
        public int ExeExternalMax { get; set; }
        public int FjMax { get; set; }
    }
}


