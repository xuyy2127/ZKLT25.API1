using ZKLT25.API.Helper;
using ZKLT25.API.IServices.Dtos;
using ZKLT25.API.Models;

namespace ZKLT25.API.IServices
{
    public interface IMenuService
    {
        Task<ResultModel<List<MenuDto>>> GetAllMenuAsync(string userName, int state, string? key);

        Task<ResultModel<bool>> CreateMenuAsync(MenuCto cto, string userName);

        Task<ResultModel<bool>> ModiyMenuAsync(MenuUto uto, string Id);

        Task<ResultModel<bool>> SetAuth(AuthCto cto);

        Task<ResultModel<string>> GetFavorites(string userName);
        Task<ResultModel<bool>> SetFavorites(string favorite, string userName);
        Task<ResultModel<bool>> AddFavorites(string userName, Favorite_Uto uto);
        Task<ResultModel<string>> GetUserName(string userId);
    }
}
