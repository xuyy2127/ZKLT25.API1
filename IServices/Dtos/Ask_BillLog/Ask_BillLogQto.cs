using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.IServices.Dtos
{
    public class Ask_BillLogQto
    {
        /// <summary>
        /// 账单详情ID
        /// </summary>
        [Required]
        public int BillDetailID { get; set; }

        /// <summary>
        /// 状态过滤
        /// </summary>
        public string? States { get; set; }
    }
}




