namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 阀体供货关系数据传输对象
    /// </summary>
    public class Ask_SuppRangeFTDto
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 供应商ID
        /// </summary>
        public int? SuppID { get; set; }

        /// <summary>
        /// 阀体ID
        /// </summary>
        public int? FTID { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        public int? lv { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string? SupplierName { get; set; }

        /// <summary>
        /// 阀体名称
        /// </summary>
        public string? FTName { get; set; }

        /// <summary>
        /// 阀体版本
        /// </summary>
        public string? FTVersion { get; set; }
    }
}