using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZKLT25.API.Helper;
using ZKLT25.API.Helper.TokenModule;
using ZKLT25.API.IServices;
using ZKLT25.API.IServices.Dtos;

namespace ZKLT25.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        public LoginController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CheckLogin(UserLogin_Qto qto)
        {
            if (string.IsNullOrWhiteSpace(qto.UserName) || string.IsNullOrWhiteSpace(qto.Password))
            {
                return Ok(ResultModel<bool>.Error("用户名和密码不能为空"));
            }
            var user = await _userService.CheckUser(qto);
            if (user == null)
                return Ok(ResultModel<bool>.Error("登录失败"));
            else
            {
                var depart = await _userService.GetDepartName(user.DepartID);
                //get token
                var token = GetToken(user.UserId, user.UserName, user.DepartID, user.Leader, user.IsManager, depart?.DepartName ?? "");
                return Ok(ResultModel<object>.Ok(new { token = token, UserName = user.UserName, DepartID = user.DepartID, Leader = user.Leader, IsManager = user.IsManager, department = depart.DepartName, UserId = user?.UserId }));
            }
        }

        [AllowAnonymous]
        [HttpPost("LoginByUserId")]
        public async Task<IActionResult> CheckLoginByUserId([FromBody] string userId)
        {
            var user = await _userService.CheckUserByUserId(userId);
            if (user == null)
                return Ok(ResultModel<bool>.Error("登录失败"));
            else
            {
                var depart = await _userService.GetDepartName(user.DepartID);
                //get token
                var token = GetToken(user.UserId, user.UserName, user.DepartID, user.Leader, user.IsManager, depart?.DepartName ?? "", 7);
                return Ok(ResultModel<object>.Ok(new { token = token, UserName = user.UserName, DepartID = user.DepartID, Leader = user.Leader, IsManager = user.IsManager, department = depart.DepartName, UserId = user.UserId }));
            }
        }

        private string GetToken(string userId, string username, string department, string leader, int? ismanager, string? departName, int expires = 1)
        {
            var token = _configuration.GetSection("Jwt").Get<JwtTokenModel>();
            token.Expires = expires;
            token.UserId = userId;
            token.UserName = username;
            token.DepartId = department;
            token.Leader = leader;
            token.IsManager = ismanager;
            token.DepartName = departName;
            return TokenHelper.CreateToken(token);
        }
    }
}
