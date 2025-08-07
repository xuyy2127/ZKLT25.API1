using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using OfficeOpenXml;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ZKLT25.API.Helper;
using ZKLT25.API.IServices;
using ZKLT25.API.IServices.Dtos;

namespace ZKLT25.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _repo;
        public MenuController(IMenuService repo)
        {
            _repo = repo;
        }

        [HttpGet("GetAllMenu")]
        public async Task<ResultModel<List<MenuDto>>> GetAllMenuAsync(string? userId, string? userName, string? key, int state = 1)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var res = await _repo.GetUserName(userId);
                if(!res.Success)
                    return ResultModel<List<MenuDto>>.Error(res.Message);
                userName = res.Data;
            }
            if (string.IsNullOrEmpty(userName))
                userName = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            return await _repo.GetAllMenuAsync(userName, state, key);
        }

        [HttpPost("AddMenu")]
        public async Task<ResultModel<bool>> AddMenuAsync([FromBody] MenuCto cto)
        {
            var userName = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return await _repo.CreateMenuAsync(cto, userName);
        }

        [HttpPost("{Id}/ModifMenu")]
        public async Task<ResultModel<bool>> ModifMenuAsync([FromRoute] string Id, [FromBody] MenuUto uto)
        {
            return await _repo.ModiyMenuAsync(uto, Id);
        }

        [HttpPost("SetAuth")]
        public async Task<ResultModel<bool>> SetAuth([FromBody] AuthCto cto)
        {
            return await _repo.SetAuth(cto);
        }

        [HttpPost("GetFavorites")]
        public async Task<ResultModel<string>> GetFavorites() =>
            await _repo.GetFavorites(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value);

        [HttpPost("SetFavorites")]
        public async Task<ResultModel<bool>> SetFavorites([FromBody] string favorites) =>
            await _repo.SetFavorites(favorites, User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value);

        [HttpPost("AddFavorites")]
        public async Task<ResultModel<bool>> AddFavorites([FromBody] Favorite_Uto uto) =>
            await _repo.AddFavorites(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, uto);


    }
}
