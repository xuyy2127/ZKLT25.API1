namespace ZKLT25.API.IServices.Dtos
{
    /// <summary>
    /// 阀体型号查询响应DTO
    /// </summary>
    public class Ask_FTListDto
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int ID { get; set; }
        
        /// <summary>
        /// 阀体型号
        /// </summary>
        public string? FTVersion { get; set; }
        
        /// <summary>
        /// 阀体名称
        /// </summary>
        public string? FTName { get; set; }
        
        /// <summary>
        /// 类型显示文本 (外购/自制)
        /// </summary>
        public string TypeText { get; set; } = "";
        
        /// <summary>
        /// 是否外购 (1=是/外购, 0=否/自制)
        /// </summary>
        public int? isWG { get; set; }
        
        /// <summary>
        /// 比例系数
        /// </summary>
        public double? ratio { get; set; }
        
        /// <summary>
        /// 是否询价显示文本 (是/否)
        /// </summary>
        public string IsAskText { get; set; } = "";
        
        /// <summary>
        /// 是否询价 (1=是, 0=否)
        /// </summary>
        public int? isAsk { get; set; }
    }
}