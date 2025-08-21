namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 附件报价查询分页
    /// </summary>
    public class Ask_DataFJDto : IDataItemDto
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 单据ID
        /// </summary>
        public int? BillID { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string? AskProjName { get; set; }

        /// <summary>
        /// 供应商名称（supplier）
        /// </summary>
        public string SuppName { get; set; }

        /// <summary>
        /// 供应商ID
        /// </summary>
        public int? SuppID { get; set; }

        /// <summary>
        /// 价格（bill price）
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// 基础价
        /// </summary>
        public double? BasicsPrice { get; set; }

        /// <summary>
        /// 加价额
        /// </summary>
        public double? AddPrice { get; set; }

        /// <summary>
        /// 询价日期
        /// </summary>
        public DateTime? AskDate { get; set; }

        /// <summary>
        /// 附件名称
        /// </summary>
        public string? FJType { get; set; }
        
        /// <summary>
        /// 附件名称ID（FJTypeId）
        /// </summary>
        public int? FJTypeId { get; set; }

        /// <summary>
        /// 附件型号
        /// </summary>
        public string? FJVersion { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int? Num { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// 是否绑定项目（0否 1是）
        /// </summary>
        public int IsPreProBind { get; set; }

        /// <summary>
        /// 文档名称
        /// </summary>
        public string? DocName { get; set; }

        /// <summary>
        /// 文档状态显示文本
        /// </summary>
        public string DocNameStatus { get; set; } = "";

        /// <summary>
        /// 文件名
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// 文件状态显示文本
        /// </summary>
        public string FileNameStatus { get; set; } = "";

        /// <summary>
        /// 价格状态显示文本
        /// </summary>
        public string PriceStatusText { get; set; } = "";

        /// <summary>
        /// 可执行的价格动作
        /// </summary>
        public string AvailableActions { get; set; } = "";

        /// <summary>
        /// 是否绑定项目显示文本
        /// </summary>
        public string IsPreProBindText { get; set; } = "";

        /// <summary>
        /// 是否失效（0=有效 1=失效）
        /// </summary>
        public int IsInvalid { get; set; }

        /// <summary>
        /// 项目交期(天)
        /// </summary>
        public string? ProjDay { get; set; }

        /// <summary>
        /// 小于10台标准交期(天)
        /// </summary>
        public string? Day1 { get; set; }

        /// <summary>
        /// 10~20台标准交期(天)
        /// </summary>
        public string? Day2 { get; set; }

        /// <summary>
        /// 备注1
        /// </summary>
        public string? Memo1 { get; set; }

        /// <summary>
        /// 单据编号
        /// </summary>
        public string? BillIDText { get; set; }

        /// <summary>
        /// 超时状态
        /// </summary>
        public int? Timeout { get; set; }
    }
}
