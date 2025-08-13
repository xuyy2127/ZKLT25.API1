namespace ZKLT25.API.IServices.Dtos
{
    public class Ask_DataFTDto
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
        /// 供应商
        /// </summary>
        public string SuppName { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public double? Price { get; set; }
        /// <summary>
        /// 基础价
        /// </summary>
        public float? BasicsPrice { get; set; }
        /// <summary>
        /// 加价金额
        /// </summary>
        public float? AddPrice { get; set; }
        /// <summary>
        /// 询价日期
        /// </summary>
        public DateTime? AskDate { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string? OrdName { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string? OrdVersion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? OrdDN { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? OrdPN { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? OrdLJ { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? OrdFG { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? OrdFT { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? OrdFNJ { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? OrdTL { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? OrdKV { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? OrdFlow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? OrdLeak { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? OrdQYDY { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? Num { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? OrdUnit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsPreProBind { get; set; }
        /// <summary>
        /// 报价文件
        /// </summary>

        /// <summary>
        /// 明细表
        /// </summary>
        public string? DocName { get; set; }
        /// <summary>
        /// 价格状态
        /// </summary>

        /// <summary>
        /// 设置价格状态
        /// </summary>

    }
}