namespace ZKLT25.API.IServices.Dtos
{
    public class Ask_BillDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public int BillID { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string? Proj { get; set; }

        /// <summary>
        /// 项目用户
        /// </summary>
        public string? ProjUser { get; set; }

        /// <summary>
        /// 开票用户
        /// </summary>
        public string? KUser { get; set; }
        
        /// <summary>
        /// 预计回复时间
        /// </summary>
        public DateTime? YSDate { get; set; }
        
        /// <summary>
        /// 状态
        /// </summary>
        public int? BillState { get; set; }

        /// <summary>
        /// 状态显示文本
        /// </summary>
        public string BillStateText { get; set; } = "";

        /// <summary>
        /// 开票日期
        /// </summary>
        public DateTime? KDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 客户名称
        /// </summary>
        public string? Customer { get; set; }
        
        /// <summary>
        /// 交期编号（来自AskDay_Bill.BillID）
        /// </summary>
        public int? DeliveryBillID { get; set; }
    }
}