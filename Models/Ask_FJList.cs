using System.ComponentModel.DataAnnotations;
using ZKLT25.API.Models.Interfaces;

namespace ZKLT25.API.Models
{
    public class Ask_FJList : IEntityWithId, IEntityWithRatio
    {
        [Key]
        public int ID { get; set; }
        
        /// <summary>
        /// 附件类型
        /// </summary>
        public string? FJType { get; set; }
        
        /// <summary>
        /// 比例系数
        /// </summary>
        public double? ratio { get; set; } = 1.0;
    }
}