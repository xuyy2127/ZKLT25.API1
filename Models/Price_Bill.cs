using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.Models
{
    public class Price_Bill
    {
        [Key]
        public int BillID { get; set; }
        
        /// <summary>
        /// 项目名称
        /// </summary>
        public string? projName { get; set; }
        
        /// <summary>
        /// 用户信息
        /// </summary>
        public string? UserInfo { get; set; }
        
        /// <summary>
        /// 数量总计
        /// </summary>
        public int? NumTotal { get; set; }
        
        /// <summary>
        /// 文档名称
        /// </summary>
        public string? DocName { get; set; }
        
        /// <summary>
        /// 客户
        /// </summary>
        public string? Customer { get; set; }
        
        /// <summary>
        /// 上次客户
        /// </summary>
        public string? LastCustomer { get; set; }
        
        /// <summary>
        /// 5S仓库ID
        /// </summary>
        public int? _5SStoreID { get; set; }
        
        /// <summary>
        /// 省份
        /// </summary>
        public string? Province { get; set; }
        
        /// <summary>
        /// 价格总计
        /// </summary>
        public float? PriceTotal { get; set; }
        
        /// <summary>
        /// 开票用户
        /// </summary>
        public string? KUser { get; set; }
        
        /// <summary>
        /// 开票日期
        /// </summary>
        public DateTime? KDate { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 状态
        /// </summary>
        public int? State { get; set; } = 0;
        
        /// <summary>
        /// 地区
        /// </summary>
        public string? Area { get; set; }
        
        /// <summary>
        /// 扩展字段1
        /// </summary>
        public string? X1 { get; set; }
        
        /// <summary>
        /// 扩展字段2
        /// </summary>
        public string? X2 { get; set; }
        
        /// <summary>
        /// 导出日期
        /// </summary>
        public DateTime? ExPortDate { get; set; }
        
        /// <summary>
        /// 文件1
        /// </summary>
        public string? File1 { get; set; }
        
        /// <summary>
        /// 文件2
        /// </summary>
        public string? File2 { get; set; }
        
        /// <summary>
        /// 说明
        /// </summary>
        public string? Exp { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        public string? Type { get; set; }
        
        /// <summary>
        /// 项目
        /// </summary>
        public string? Proj { get; set; }
        
        /// <summary>
        /// 执行用户
        /// </summary>
        public string? DoUser { get; set; }
        
        /// <summary>
        /// 含税价格
        /// </summary>
        public float? PriceHT { get; set; }
        
        /// <summary>
        /// JSON数据
        /// </summary>
        public string? JsonData { get; set; }
        
        /// <summary>
        /// JSON数据2
        /// </summary>
        public string? JsonData2 { get; set; }
        
        /// <summary>
        /// 账单类型
        /// </summary>
        public int? BillType { get; set; } = 0;
        
        /// <summary>
        /// 旧账单ID
        /// </summary>
        public string? OldBillID { get; set; }
        
        /// <summary>
        /// 报价ID
        /// </summary>
        public int? QuotedPriceId { get; set; }
        
        /// <summary>
        /// BPM账单ID
        /// </summary>
        public int? BpmBillID { get; set; }
    }
}




