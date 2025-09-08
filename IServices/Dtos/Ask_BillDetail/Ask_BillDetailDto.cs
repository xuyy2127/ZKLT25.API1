namespace ZKLT25.API.IServices.Dtos
{
    public class Ask_BillDetailDto : IHasBillId
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int ID { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// 价格（billprice）
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 通径
        /// </summary>
        public string? DN { get; set; }

        /// <summary>
        /// 压力
        /// </summary>
        public string? PN { get; set; }

        /// <summary>
        /// 连接标准
        /// </summary>
        public string? LJ { get; set; }

        /// <summary>
        /// 上法兰连接方式
        /// </summary>
        public string? FG { get; set; }

        /// <summary>
        /// 阀体材质
        /// </summary>
        public string? FT { get; set; }

        /// <summary>
        /// 阀内件材质
        /// </summary>
        public string? FNJ { get; set; }

        /// <summary>
        /// 介质/温度
        /// </summary>
        public string? ordMed { get; set; }

        /// <summary>
        /// KV值
        /// </summary>
        public string? OrdKV { get; set; }

        /// <summary>
        /// 法兰
        /// </summary>
        public string? ordFW { get; set; }

        /// <summary>
        /// 泄漏等级
        /// </summary>
        public string? ordLeak { get; set; }

        /// <summary>
        /// 气源压力/气源
        /// </summary>
        public string? ordQY { get; set; }

        /// <summary>
        /// 填料材质
        /// </summary>
        public string? TL { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? State { get; set; }

        /// <summary>
        /// 开票日期（billprice）
        /// </summary>
        public DateTime? KDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Memo { get; set; }

        /// <summary>
        /// 报价备注
        /// </summary>
        public string? CGPriceMemo { get; set; }

        /// <summary>
        /// 报价回复备注
        /// </summary>
        public string? Remarks { get; set; }
        
        /// <summary>
        /// 数量
        /// </summary>
        public double? Num { get; set; }
        
        /// <summary>
        /// 供应商ID（billprice）
        /// </summary>
        public int? SuppID { get; set; }
        
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string? SupplierName { get; set; }
        
        /// <summary>
        /// 基础价格（billprice）
        /// </summary>
        public double? BasicsPrice { get; set; }
        
        /// <summary>
        /// 附加价格（billprice）
        /// </summary>
        public double? AddPrice { get; set; }
        
        /// <summary>
        /// 开票用户（billprice）
        /// </summary>
        public string? KUser { get; set; }

        /// <summary>
        /// 询价单据ID
        /// </summary>
        public int? BillID { get; set; }

        /// <summary>
        /// 明细表文件名（来自 Ask_Bill.DocName）
        /// </summary>
        public string? DocName { get; set; }

        public string DocPath { get; set; }    // 逻辑/相对路径

        public string FilePath { get; set; }   // 可直接下载的 URL
    }
}