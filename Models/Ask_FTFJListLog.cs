using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.Models
{
    public class Ask_FTFJListLog
    {
        [Key]
        public int ID { get; set; }
        
        /// <summary>
        /// 关联主表ID
        /// </summary>
        public int MainID { get; set; }
        
        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; } = "";
        
        /// <summary>
        /// 零件类型
        /// </summary>
        public string? PartType { get; set; }
        
        /// <summary>
        /// 零件型号
        /// </summary>
        public string? PartVersion { get; set; }
        
        /// <summary>
        /// 零件名称
        /// </summary>
        public string? PartName { get; set; }
        
        /// <summary>
        /// 比例系数
        /// </summary>
        public double Ratio { get; set; }
        
        /// <summary>
        /// 创建用户
        /// </summary>
        public string CreateUser { get; set; } = "";
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
} 