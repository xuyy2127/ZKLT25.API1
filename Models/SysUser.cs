using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZKLT25.API.Models
{
    public class SysUser
    {
        [Key]
        public string? UserName { get; set; }
        public string? UserId { get; set; }
        public string? UserPwd { get; set; }
        public char? UserGender { get; set; }
        public int? UserAge { get; set; }
        public DateTime? UserBirth { get; set; }
        public string? DepartID { get; set; }
        public string? Job { get; set; }
        public string? JobID { get; set; }
        public string? UserMail { get; set; }
        public string? UserShort { get; set; }
        public string? UserCall { get; set; }
        public string? UserPhone { get; set; }
        public string? UserAddress { get; set; }
        public DateTime? UserCome { get; set; } = DateTime.Now;
        public string? UserHeader { get; set; }
        public string? UserState { get; set; }
        public string? UserMac { get; set; }
        public int? isLocked { get; set; } = 1;
        public string? OpenID { get; set; }
        public string? ProductOpenId { get; set; }
        public string? WxInfo { get; set; }
        public string? EmployeeNum { get; set; }
        public string? Cid { get; set; }
        public int? Deptd2 { get; set; }
        public int? Deptd3 { get; set; }
        public int? Deptd4 { get; set; }
        public int? isLeader { get; set; }
        public string? supdepname { get; set; }
        public string? departmentname { get; set; }
        public string? subcompanyname { get; set; }
        public string? namepath { get; set; }

        public string? Leader { get; set; }
        public int? IsManager { get; set; }
    }
}
