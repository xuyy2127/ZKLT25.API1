namespace ZKLT25.API.IServices.Dtos
{
    public class Ask_DataFTDto
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int ID { get; set; }
        
        /// <summary>
        /// 交期编号
        /// </summary>
        public int? BillID { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string? AskProjName { get; set; }
        /// <summary>
        /// 供应商名称（spplier
        /// </summary>
        public string SuppName { get; set; }
        /// <summary>
        /// 单价（billprice
        /// </summary>
        public double? Price { get; set; }
                /// <summary>
        /// 基础价
        /// </summary>
        public double? BasicsPrice { get; set; }
        /// <summary>
        /// 加价金额
        /// </summary>
        public double? AddPrice { get; set; }
        /// <summary>
        /// 询价日期
        /// </summary>
        public DateTime? AskDate { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string? OrdName { get; set; }
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
        /// 法兰标准
        /// </summary>
        public string? OrdLJ { get; set; }
        /// <summary>
        /// 上阀盖形式
        /// </summary>
        public string? OrdFG { get; set; }
        /// <summary>
        /// 阀体材质
        /// </summary>
        public string? OrdFT { get; set; }
        /// <summary>
        /// 阀内件材质
        /// </summary>
        public string? OrdFNJ { get; set; }
        /// <summary>
        /// 填料材质
        /// </summary>
        public string? OrdTL { get; set; }
        /// <summary>
        /// 额定KV值
        /// </summary>
        public string? OrdKV { get; set; }
        /// <summary>
        /// 流量特性
        /// </summary>
        public string? OrdFlow { get; set; }
        /// <summary>
        /// 泄露等级
        /// </summary>
        public string? OrdLeak { get; set; }
        /// <summary>
        /// 气源压力
        /// </summary>
        public string? OrdQYDY { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int? Num { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string? OrdUnit { get; set; }
        /// <summary>
        /// 有无绑定项目
        /// </summary>
        public int IsPreProBind { get; set; }
        /// <summary>
        /// 明细表
        /// </summary>
        public string? DocName { get; set; }
        
        /// <summary>
        /// 明细表状态显示（有/无）
        /// </summary>
        public string DocNameStatus { get; set; } = "";
        
        /// <summary>
        /// 报价文件
        /// </summary>
        public string? FileName { get; set; }
        
        /// <summary>
        /// 报价文件状态显示（有/无）
        /// </summary>
        public string FileNameStatus { get; set; } = "";
        
        /// <summary>
        /// 价格状态显示文本
        /// </summary>
        public string PriceStatusText { get; set; } = "";
        
        /// <summary>
        /// 可执行的价格状态操作
        /// </summary>
        public string AvailableActions { get; set; } = "";
        
        /// <summary>
        /// 项目绑定状态显示文本
        /// </summary>
        public string IsPreProBindText { get; set; } = "";
        
        /// <summary>
        /// 数据有效性标记（1=有效，0=无效）
        /// </summary>
        public int IsInvalid { get; set; }
        
        /// <summary>
        /// 超时状态（用于计算价格状态和可用操作）
        /// </summary>
        public int? Timeout { get; set; }
    }
}