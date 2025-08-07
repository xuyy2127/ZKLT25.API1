using Microsoft.EntityFrameworkCore;
using ZKLT25.API.EntityFrameworkCore;
using ZKLT25.API.IServices;
using ZKLT25.API.IServices.Dtos;
using ZKLT25.API.Models;

namespace ZKLT25.API.Services
{
    public class UserService : IUserService
    {
        private AppDbContext _db;
        public UserService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<SysUser> CheckUser(UserLogin_Qto dto)
        {
            return await _db.SysUser.FirstOrDefaultAsync(m => (m.UserName == dto.UserName || m.UserPhone == dto.UserName) && m.UserPwd == dto.Password);
        }
        public async Task<SysDepart> GetDepartName(string DepartId)
        {
            return await _db.SysDepart.FirstOrDefaultAsync(a => a.DepartID == DepartId);
        }

        public async Task<SysUser> CheckUserByUserId(string userId)
        {
            return await _db.SysUser.FirstOrDefaultAsync(a => a.UserId == userId);
        }
    }
}
