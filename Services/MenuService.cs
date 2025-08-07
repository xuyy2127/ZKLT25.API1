using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ZKLT25.API.EntityFrameworkCore;
using ZKLT25.API.Helper;
using ZKLT25.API.IServices;
using ZKLT25.API.IServices.Dtos;
using ZKLT25.API.Models;

namespace ZKLT25.API.Services
{
    public class MenuService : IMenuService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public MenuService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        /// <summary>
        /// 获取菜单列表 - 接口方法
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task<ResultModel<List<MenuDto>>> GetAllMenuAsync(string userName, int state, string? key)
        {
            try
            {
                var parentId = new List<string?>();
                List<MenuDto> res = [];
                var auths = await _db.SysAuth.Where(a => a.UserName == userName).ToListAsync();
                //step1 获取全部菜单 1,2:mes 3,4:pad
                var query = _db.Base_SysMenu.Where(a => a.DeleteMark != 1);
                if (!string.IsNullOrEmpty(key))
                {
                    var queryres = await query.Where(a => a.Menu_Type == "page" && (a.Menu_Name.Contains(key) || a.NavigateUrl.Contains(key))).ToListAsync();
                    var result = _mapper.Map<List<MenuDto>>(queryres);
                    result.ForEach(r => r.IsAuth = auths.Any(a => a.MenuID == r.Id) ? 1 : 0);
                    return ResultModel<List<MenuDto>>.Ok(result);
                }
                var menus = await query.OrderBy(m => m.SortCode).ToListAsync();
                if (state == 1 || state == 2)
                    menus = menus.Where(m => m.State == 0).ToList();
                else if (state == 3 || state == 4)
                    menus = menus.Where(m => m.State == 1).ToList();

                var dtos = _mapper.Map<List<MenuDto>>(menus);
                //step2 获取权限 1.3:个人菜单
                //var auths = await _db.SysAuth.Where(a => a.UserName == userName).ToListAsync();
                var dic = auths.GroupBy(a => new { a.MenuID, a.UserName }).ToDictionary(k => k.Key.MenuID.ToString(), v => v.Max(g => g.Favorites.HasValue ? g.Favorites.Value : 0));
                dtos.ForEach(d =>
                {
                    if (dic.ContainsKey(d.Id))
                    {
                        d.IsAuth = 1;
                        d.Favorites = dic[d.Id].ToString();
                    }
                    if (d.DeleteMark == 2)
                        d.IsAuth = 1;
                });

                if (state == 2 || state == 4)
                {
                    parentId = menus.Where(m => (auths.Select(a => a.MenuID).Contains(m.Id) || m.DeleteMark == 2) && m.DeleteMark != 1)
                        .Select(m => m.ParentId)
                        .Distinct()
                        .ToList();

                    res = _mapper.Map<List<MenuDto>>(menus.Where(m => parentId.Contains(m.Id)));
                    dtos = dtos.Where(m => auths.Select(a => a.MenuID).Contains(m.Id) || m.DeleteMark == 2).ToList();
                }
                else
                {
                    res = _mapper.Map<List<MenuDto>>(menus.Where(m => m.Menu_Type == "menu"));
                }

                foreach (var menu in res)
                {
                    menu.Children = dtos.Where(a => a.ParentId == menu.Id).ToList();
                }

                return ResultModel<List<MenuDto>>.Ok(res);
            }
            catch (Exception ex)
            {
                return ResultModel<List<MenuDto>>.Error($"获取菜单失败:{ex.Message}");
            }
        }

        /// <summary>
        /// 新增菜单 - 接口方法
        /// </summary>
        /// <param name="cto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<ResultModel<bool>> CreateMenuAsync(MenuCto cto, string userName)
        {
            try
            {
                if (cto.Method == 1) //新增
                {
                    var menu = _mapper.Map<Base_SysMenu>(cto);
                    menu.Id = Guid.NewGuid().ToString().ToUpper();
                    menu.CreateUserName = userName;
                    menu.CreateDate = DateTime.Now;
                    _db.Add(menu);

                }
                else
                {
                    var menu = await _db.Base_SysMenu.FirstOrDefaultAsync(a => a.Id == cto.MenuId);
                    if (menu == null)
                        return ResultModel<bool>.Error("数据不存在");

                    if (cto.Method == 2) //修改
                        CommonHelper.UpdateEntity(menu, cto);
                    else //删除
                        _db.Remove(menu);
                }

                await _db.SaveChangesAsync();
                return ResultModel<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"新增失败:{ex.Message}");
            }
        }

        /// <summary>
        /// 修改菜单 - 接口方法
        /// </summary>
        /// <param name="uto"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<ResultModel<bool>> ModiyMenuAsync(MenuUto uto, string Id)
        {
            try
            {
                var menu = await _db.Base_SysMenu.FindAsync(Id);
                if (menu == null)
                    return ResultModel<bool>.Error("菜单不存在");
                CommonHelper.UpdateEntity(menu, uto);
                await _db.SaveChangesAsync();
                return ResultModel<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"菜单修改失败:{ex.Message}");
            }
        }

        /// <summary>
        /// 配置菜单权限 - 接口方法
        /// </summary>
        /// <param name="cto"></param>
        /// <returns></returns>
        public async Task<ResultModel<bool>> SetAuth(AuthCto cto)
        {
            try
            {
                if (cto.Method == 1)
                {
                    var auth = new SysAuth
                    {
                        MenuID = cto.MenuId,
                        UserName = cto.UserName,
                        PageID = "",
                        AuthOrder = 1,
                        SetDate = DateTime.Now,
                        Favorites = 0
                    };
                    _db.Add(auth);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    await _db.SysAuth.Where(a => a.UserName == cto.UserName && a.MenuID == cto.MenuId)
                        .ExecuteDeleteAsync();
                }

                return ResultModel<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"配置菜单权限失败:{ex.Message}");
            }

        }

        /// <summary>
        /// 获取用户菜单收藏列表 - 接口方法
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<ResultModel<string>> GetFavorites(string userName)
        {
            var res = await _db.Favorite.FirstOrDefaultAsync(a => a.UserName == userName);
            return ResultModel<string>.Ok(res?.Favorites ?? "");
        }

        /// <summary>
        /// 收藏夹排序 - 接口方法
        /// </summary>
        /// <param name="favorite"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<ResultModel<bool>> SetFavorites(string favorite, string userName)
        {
            try
            {
                var res = await _db.Favorite.FirstOrDefaultAsync(a => a.UserName == userName);
                if (res == null)
                    _db.Add(new Favorite { UserName = userName, Favorites = favorite });
                else
                    res.Favorites = favorite;

                await _db.SaveChangesAsync();
                return ResultModel<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"设置收藏夹失败:{ex.Message}");
            }

        }

        /// <summary>
        /// 加入收藏夹 - 接口方法
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="MenuId"></param>
        /// <param name="favorite"></param>
        /// <returns></returns>
        public async Task<ResultModel<bool>> AddFavorites(string userName, Favorite_Uto uto)
        {
            try
            {
                await _db.SysAuth.Where(a => a.UserName == userName && a.MenuID == uto.MenuId).ExecuteUpdateAsync(a => a.SetProperty(x => x.Favorites, uto.Favorites));

                return ResultModel<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"加入收藏夹失败:{ex.Message}");
            }
        }



        /// <summary>
        /// 获取用户编号
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ResultModel<string>> GetUserName(string userId)
        {
            try
            {
                var user = await _db.SysUser.FirstOrDefaultAsync(a => a.UserId == userId);
                if (user == null)
                    throw new Exception("用户编号不存在");
                return ResultModel<string>.Ok(user.UserName);
            }
            catch (Exception ex)
            {
                return ResultModel<string>.Error($"用户编号错误:{ex.Message}");
            }
        }
    }
}
