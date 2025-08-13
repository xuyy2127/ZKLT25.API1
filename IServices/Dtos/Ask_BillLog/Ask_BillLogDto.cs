namespace ZKLT25.API.IServices.Dtos
{
    public class Ask_BillLogDto
    {
        /// <summary>
        /// 操作用户
        /// </summary>
        public string? KUser { get; set; }

        /// <summary>
        /// 操作日期
        /// </summary>
        public DateTime? KDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string? State { get; set; }

        /// <summary>
        /// 价格（BillPrice表）
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// 备注（BillPrice表）
        /// </summary>
        public string? Remarks { get; set; }
    }
}
