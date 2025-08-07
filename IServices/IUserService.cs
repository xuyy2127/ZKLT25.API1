using ZKLT25.API.IServices.Dtos;
using ZKLT25.API.Models;

namespace ZKLT25.API.IServices
{
    public interface IUserService
    {
        Task<SysUser> CheckUser(UserLogin_Qto qto);
        Task<SysDepart> GetDepartName(string DepartId);
        Task<SysUser> CheckUserByUserId(string userId);
    }
}
