using System.ComponentModel.DataAnnotations;

namespace ZKLT25.API.Helper.Model
{
    public class IModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CreateDateStr
        {
            get
            {
                return CreateDate.ToString("yyyy-MM-dd");
            }
        }
    }
}
