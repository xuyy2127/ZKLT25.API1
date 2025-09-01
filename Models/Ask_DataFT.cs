using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.Models
{
    public class Ask_DataFT : IDataFTEntity, ITimeoutBindable
    {
        [Key]
        public int ID { get; set; }
        
        /// <summary>
        /// 来源
        /// </summary>
        public string? Source { get; set; } = "询价待办";
        
        /// <summary>
        /// 询价日期
        /// </summary>
        public DateTime? AskDate { get; set; }
        
        /// <summary>
        /// 询价项目名称
        /// </summary>
        public string? AskProjName { get; set; }
        
        /// <summary>
        /// 订单编号
        /// </summary>
        public string? OrdNum { get; set; }
        
        /// <summary>
        /// 订单介质
        /// </summary>
        public string? OrdMed { get; set; }
        
        /// <summary>
        /// 订单名称
        /// </summary>
        public string? OrdName { get; set; }
        
        /// <summary>
        /// 订单版本
        /// </summary>
        public string? OrdVersion { get; set; }
        
        /// <summary>
        /// 订单DN
        /// </summary>
        public string? OrdDN { get; set; }
        
        /// <summary>
        /// 订单PN
        /// </summary>
        public string? OrdPN { get; set; }
        
        /// <summary>
        /// 订单连接
        /// </summary>
        public string? OrdLJ { get; set; }
        
        /// <summary>
        /// 订单阀盖
        /// </summary>
        public string? OrdFG { get; set; }
        
        /// <summary>
        /// 订单阀体
        /// </summary>
        public string? OrdFT { get; set; }
        
        /// <summary>
        /// 订单阀内件
        /// </summary>
        public string? OrdFNJ { get; set; }
        
        /// <summary>
        /// 订单填料
        /// </summary>
        public string? OrdTL { get; set; }
        
        /// <summary>
        /// 订单KV值
        /// </summary>
        public string? OrdKV { get; set; }
        
        /// <summary>
        /// 订单流向
        /// </summary>
        public string? OrdFlow { get; set; }
        
        /// <summary>
        /// 订单泄漏
        /// </summary>
        public string? OrdLeak { get; set; }
        
        /// <summary>
        /// 订单驱动
        /// </summary>
        public string? OrdQYDY { get; set; }
        
        /// <summary>
        /// 订单单位
        /// </summary>
        public string? OrdUnit { get; set; }
        
        /// <summary>
        /// 数量
        /// </summary>
        public int? Num { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string? Memo { get; set; }
        
        /// <summary>
        /// 询价要求
        /// </summary>
        public string? AskRequire { get; set; }
        
        /// <summary>
        /// 价格
        /// </summary>
        public double? Price { get; set; }
        
        /// <summary>
        /// 供应商
        /// </summary>
        public int? Supplier { get; set; }
        
        /// <summary>
        /// 项目天数
        /// </summary>
        public string? ProjDay { get; set; }
        
        /// <summary>
        /// 天数1
        /// </summary>
        public string? Day1 { get; set; }
        
        /// <summary>
        /// 天数2
        /// </summary>
        public string? Day2 { get; set; }
        
        /// <summary>
        /// 天数3
        /// </summary>
        public string? Day3 { get; set; }
        
        /// <summary>
        /// 备注1
        /// </summary>
        public string? Memo1 { get; set; }
        
        /// <summary>
        /// 执行用户
        /// </summary>
        public string? DoUser { get; set; }
        
        /// <summary>
        /// 执行日期
        /// </summary>
        public DateTime? DoDate { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 价格比例
        /// </summary>
        public double? PriceRatio { get; set; }
        
        /// <summary>
        /// 账单详情ID
        /// </summary>
        public int? BillDetailID { get; set; }
        
        /// <summary>
        /// 是否预生产绑定
        /// </summary>
        public int IsPreProBind { get; set; } = 0;
        
        /// <summary>
        /// 超时
        /// </summary>
        public int? Timeout { get; set; } = -1;
    }
}
