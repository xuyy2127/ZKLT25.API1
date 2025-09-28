namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 采购成本（Ask_CGPrice）简要DTO
    /// 说明：ProjName/Customer/DoUser/KUser/KDate 通过 Ask_CGPrice.Id 与 Price_Bill.BillID 关联取得
    /// </summary>
    public class Ask_CGPriceDto
    {
        /// <summary>
        /// 类型（Ask_CGPrice.Type）
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// 型号（Ask_CGPrice.Version）
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// 气源压力（Ask_CGPrice.ordQY）
        /// </summary>
        public string? ordQY { get; set; }

        /// <summary>
        /// 数量（Ask_CGPrice.Num）
        /// </summary>
        public double? Num { get; set; }

        /// <summary>
        /// 口径（Ask_CGPrice.DN）
        /// </summary>
        public string? DN { get; set; }

        /// <summary>
        /// 压力（Ask_CGPrice.PN）
        /// </summary>
        public string? PN { get; set; }

        /// <summary>
        /// 价格（Ask_CGPrice.Price）
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// 加价（Ask_CGPrice.AddPrice）
        /// </summary>
        public double? AddPrice { get; set; }

        /// <summary>
        /// 价格备注（Ask_CGPrice.PriceMemo）
        /// </summary>
        public string? PriceMemo { get; set; }

        /// <summary>
        /// 项目名称（Price_Bill.ProjName）
        /// </summary>
        public string? ProjName { get; set; }

        /// <summary>
        /// 项目（Price_Bill.Proj）
        /// </summary>
        public string? Proj { get; set; }

        /// <summary>
        /// 客户（Price_Bill.Customer）
        /// </summary>
        public string? Customer { get; set; }

        /// <summary>
        /// 执行人（Price_Bill.DoUser）
        /// </summary>
        public string? DoUser { get; set; }

        /// <summary>
        /// 询价用户（Price_Bill.KUser）
        /// </summary>
        public string? KUser { get; set; }

        /// <summary>
        /// 询价日期（Price_Bill.KDate）
        /// </summary>
        public DateTime? KDate { get; set; }

        /// <summary>
        /// 派生：是否可查询历史（State==2）
        /// </summary>
        public bool CanQuery { get; set; }

        /// <summary>
        /// 项目数：同一 Version 下不同项目（ProjName）的数量
        /// </summary>
        public int ProjectCount { get; set; }

        public int? SuppID { get; set; }
    }
}
