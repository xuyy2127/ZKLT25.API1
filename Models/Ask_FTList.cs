using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.Models
{
    public class Ask_FTList : IEntityWithId, IEntityWithRatio
    {
        [Key]
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
        /// 是否外购 (1=是/外购, 0=否/自制)
        /// </summary>
        public int? isWG { get; set; } = 1;
        
        /// <summary>
        /// 比例系数
        /// </summary>
        public double? ratio { get; set; } = 1.0;
        
        /// <summary>
        /// 是否询价 (1=是, 0=否)
        /// </summary>
        public int? isAsk { get; set; }
    }
}