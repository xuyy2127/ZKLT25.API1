using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ZKLT25.API.EntityFrameworkCore;
using ZKLT25.API.Helper;
using ZKLT25.API.IServices;
using ZKLT25.API.IServices.Dtos;
using ZKLT25.API.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace ZKLT25.API.Services
{
    public class AskService : IAskService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public AskService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }



        /// <summary>
        /// 记录修改日志
        /// </summary>
        /// <param name="mainId">主表ID</param>
        /// <param name="dataType">数据类型（"阀体"或"附件"）</param>
        /// <param name="partType">零件类型</param>
        /// <param name="partVersion">零件型号</param>
        /// <param name="partName">零件名称</param>
        /// <param name="ratio">比例系数</param>
        /// <param name="currentUser">当前用户</param>
        private async Task AddLogAsync(int mainId, string dataType, string? partType, string? partVersion, string? partName, double ratio, string? currentUser)
        {
            try
            {

                var log = new Ask_FTFJListLog
                {
                    MainID = mainId,
                    DataType = dataType,
                    PartType = partType,
                    PartVersion = partVersion,
                    PartName = partName,
                    Ratio = ratio,
                    CreateUser = currentUser ?? "系统用户",
                    CreateDate = DateTime.Now
                };

                _db.Ask_FTFJListLog.Add(log);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"记录日志失败: {ex.Message}");
            }
        }

        #region 阀体型号维护
        /// <summary>
        /// 分页查询阀体型号列表
        /// </summary>
        public async Task<ResultModel<PaginationList<Ask_FTListDto>>> GetFTPagedListAsync(Ask_FTListQto qto)
        {
            try
            {
                var query = _db.Ask_FTList.AsQueryable();

                // 关键字搜索：型号和名称
                if (!string.IsNullOrWhiteSpace(qto.Keyword))
                {
                    query = query.Where(x => x.FTVersion.Contains(qto.Keyword) || x.FTName.Contains(qto.Keyword));
                }

                // 是否外购筛选
                if (qto.isWG.HasValue)
                {
                    query = query.Where(x => x.isWG == qto.isWG.Value);
                }

                // 是否询价筛选
                if (qto.isAsk.HasValue)
                {
                    query = query.Where(x => x.isAsk == qto.isAsk.Value);
                }

                // 排序
                query = query.OrderBy(x => x.ID);

                // 分页
                var result = await PaginationList<Ask_FTListDto>.CreateAsync(qto.PageNumber, qto.PageSize, query.ProjectTo<Ask_FTListDto>(_mapper.ConfigurationProvider));
                return ResultModel<PaginationList<Ask_FTListDto>>.Ok(result);
            }
            catch (Exception ex)
            {
                return ResultModel<PaginationList<Ask_FTListDto>>.Error($"查询失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 创建新阀体型号
        /// </summary>
        public async Task<ResultModel<bool>> CreateFTAsync(Ask_FTListCto cto)
        {
            try
            {
                var existsResult = await ExistsFTAsync(cto.FTVersion);
                if (existsResult.Data)
                {
                    return ResultModel<bool>.Error("阀体型号已存在");
                }

                var entity = _mapper.Map<Ask_FTList>(cto);

                _db.Ask_FTList.Add(entity);
                await _db.SaveChangesAsync();

                var model = ResultModel<bool>.Ok(true);
                model.Warning = GetRatioWarning(cto.isWG, cto.ratio);
                return model;
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"创建失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 更新阀体型号
        /// </summary>
        public async Task<ResultModel<bool>> UpdateFTAsync(int id, Ask_FTListUto uto, string? currentUser)
        {
            try
            {
                var entity = await _db.Ask_FTList.FindAsync(id);
                if (entity == null)
                {
                    return ResultModel<bool>.Error("记录不存在");
                }

                var existsResult = await ExistsFTAsync(uto.FTVersion, id);
                if (existsResult.Data)
                {
                    return ResultModel<bool>.Error("阀体型号已存在");
                }

                _mapper.Map(uto, entity);

                await _db.SaveChangesAsync();

                // 记录修改日志
                await AddLogAsync(
                    mainId: id,
                    dataType: "阀体",
                    partType: "阀体",
                    partVersion: uto.FTVersion,
                    partName: uto.FTName,
                    ratio: uto.ratio ?? 1.0, // 如果为空则使用默认值1.0
                    currentUser: currentUser
                );

                var model = ResultModel<bool>.Ok(true);
                model.Warning = GetRatioWarning(uto.isWG, uto.ratio);
                return model;
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"更新失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 删除阀体型号
        /// </summary>
        public async Task<ResultModel<bool>> DeleteFTAsync(int id)
        {
            try
            {
                var entity = await _db.Ask_FTList.FindAsync(id);
                if (entity == null)
                {
                    return ResultModel<bool>.Error("记录不存在");
                }

                _db.Ask_FTList.Remove(entity);
                await _db.SaveChangesAsync();

                return ResultModel<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"删除失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 检查阀体型号是否存在
        /// </summary>
        public async Task<ResultModel<bool>> ExistsFTAsync(string ftVersion, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ftVersion))
                {
                    return ResultModel<bool>.Error("阀体型号不能为空");
                }

                var query = _db.Ask_FTList.Where(x => x.FTVersion == ftVersion);

                if (excludeId.HasValue)
                {
                    query = query.Where(x => x.ID != excludeId.Value);
                }

                return ResultModel<bool>.Ok(await query.AnyAsync());
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"查询失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 批量更新系数
        /// </summary>
        public async Task<ResultModel<bool>> BatchUpdateFTRatioAsync(List<int> ids, double ratio, string? currentUser)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return ResultModel<bool>.Error("请选择要更新的记录");
                }

                var entities = await _db.Ask_FTList.Where(x => ids.Contains(x.ID)).ToListAsync();
                if (!entities.Any())
                {
                    return ResultModel<bool>.Error("未找到要更新的记录");
                }

                string? warning = null;
                // 批量更新系数并记录日志
                foreach (var entity in entities)
                {
                    entity.ratio = ratio;

                    if (warning == null)
                    {
                        warning = GetRatioWarning(entity.isWG, entity.ratio);
                    }

                    // 记录修改日志
                    await AddLogAsync(
                        mainId: entity.ID,
                        dataType: "阀体",
                        partType: "阀体",
                        partVersion: entity.FTVersion,
                        partName: entity.FTName,
                        ratio: ratio,
                        currentUser: currentUser
                    );
                }

                await _db.SaveChangesAsync();

                var model = ResultModel<bool>.Ok(true);
                model.Warning = warning;
                return model;
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"批量更新失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="isWG">是否外购 (1=外购, 0=自制)</param>
        /// <param name="ratio">价格系数</param>
        /// <returns>警告信息，如果没有警告则返回null</returns>
        private string? GetRatioWarning(int? isWG, double? ratio)
        {
            if (!ratio.HasValue || !isWG.HasValue)
                return null;

            if (isWG == 1 && ratio <= 1)
            {
                return "外购产品的系数小于等于1，请确认是否正确";
            }
            return null;
        }
        #endregion

        #region 附件系数维护
        /// <summary>
        /// 分页查询附件列表
        /// </summary>
        public async Task<ResultModel<PaginationList<Ask_FJListDto>>> GetFJPagedListAsync(Ask_FJListQto qto)
        {
            try
            {
                var query = _db.Ask_FJList.AsQueryable();

                // 关键字搜索
                if (!string.IsNullOrWhiteSpace(qto.Keyword))
                {
                    query = query.Where(x => x.FJType != null && x.FJType.Contains(qto.Keyword));
                }

                // 排序
                query = query.OrderBy(x => x.ID);

                // 分页
                var result = await PaginationList<Ask_FJListDto>.CreateAsync(qto.PageNumber, qto.PageSize, query.ProjectTo<Ask_FJListDto>(_mapper.ConfigurationProvider));
                return ResultModel<PaginationList<Ask_FJListDto>>.Ok(result);
            }
            catch (Exception ex)
            {
                return ResultModel<PaginationList<Ask_FJListDto>>.Error($"查询失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 批量更新系数
        /// </summary>
        public async Task<ResultModel<bool>> BatchUpdateFJRatioAsync(List<int> ids, double ratio, string? currentUser)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return ResultModel<bool>.Error("请选择要更新的记录");
                }

                var entities = await _db.Ask_FJList.Where(x => ids.Contains(x.ID)).ToListAsync();
                if (!entities.Any())
                {
                    return ResultModel<bool>.Error("未找到要更新的记录");
                }

                // 批量更新系数并记录日志
                foreach (var entity in entities)
                {
                    entity.ratio = ratio;

                    // 记录修改日志
                    await AddLogAsync(
                        mainId: entity.ID,
                        dataType: "附件",
                        partType: "附件",
                        partVersion: entity.FJType,
                        partName: entity.FJType,
                        ratio: ratio,
                        currentUser: currentUser
                    );
                }

                await _db.SaveChangesAsync();

                return ResultModel<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"批量更新失败：{ex.Message}");
            }
        }
        #endregion

        #region 阀体 / 附件操作日志查询
        /// <summary>
        /// 分页查询阀体 / 附件日志
        /// </summary>
        public async Task<ResultModel<PaginationList<Ask_FTFJListLogDto>>> GetFTFJLogPagedListAsync(Ask_FTFJListLogQto qto)
        {
            try
            {
                // 校验查询参数 & 分页
                if (!qto.MainID.HasValue || string.IsNullOrWhiteSpace(qto.DataType))
                    return ResultModel<PaginationList<Ask_FTFJListLogDto>>.Error("查询日志时请同时提供 MainID 和 DataType。");

                var query = _db.Ask_FTFJListLog.AsNoTracking()
                    .Where(x => x.MainID == qto.MainID && x.DataType == qto.DataType);

                query = query.OrderByDescending(x => x.CreateDate);

                var list = await PaginationList<Ask_FTFJListLogDto>.CreateAsync(qto.PageNumber, qto.PageSize, query.ProjectTo<Ask_FTFJListLogDto>(_mapper.ConfigurationProvider));
                return ResultModel<PaginationList<Ask_FTFJListLogDto>>.Ok(list);
            }
            catch (Exception ex)
            {
                return ResultModel<PaginationList<Ask_FTFJListLogDto>>.Error("查询失败，请重试。");
            }
        }
        #endregion

        #region 供应商维护
        /// <summary>
        /// 分页查询供应商信息
        /// </summary>
        public async Task<ResultModel<PaginationList<Ask_SupplierDto>>> GetSPPagedListAsync(Ask_SupplierQto qto)
        {
            try
            {
                var query = _db.Ask_Supplier.AsQueryable();

                // 关键字搜索：供应商名称
                if (!string.IsNullOrWhiteSpace(qto.Keyword))
                {
                    query = query.Where(x => x.SuppName.Contains(qto.Keyword));
                }

                // 排序
                query = query.OrderBy(x => x.ID);

                // 分页
                var result = await PaginationList<Ask_SupplierDto>.CreateAsync(qto.PageNumber, qto.PageSize, query.ProjectTo<Ask_SupplierDto>(_mapper.ConfigurationProvider));
                return ResultModel<PaginationList<Ask_SupplierDto>>.Ok(result);
            }
            catch (Exception ex)
            {
                return ResultModel<PaginationList<Ask_SupplierDto>>.Error($"查询失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 创建供应商
        /// </summary>
        public async Task<ResultModel<bool>> CreateSPAsync(Ask_SupplierCto cto, string? currentUser)
        {
            try
            {
                var existsResult = await ExistsSPAsync(cto.SuppName);
                if (existsResult.Data)
                {
                    return ResultModel<bool>.Error("供应商已存在");
                }

                var entity = _mapper.Map<Ask_Supplier>(cto);
                entity.KDate = DateTime.Now;
                entity.KUser = currentUser ?? "系统用户";

                _db.Ask_Supplier.Add(entity);
                await _db.SaveChangesAsync();
                
                return ResultModel<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"创建失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 更新供应商信息
        /// </summary>
        public async Task<ResultModel<bool>> UpdateSPAsync(int id, Ask_SupplierUto uto, string? currentUser)
        {
            try
            {

                var entity = await _db.Ask_Supplier.FindAsync(id);
                if (entity == null)
                {
                    return ResultModel<bool>.Error("记录不存在");
                }

                var existsResult = await ExistsSPAsync(uto.SuppName, id);
                if (existsResult.Data)
                {
                    return ResultModel<bool>.Error("该供应商已存在");
                }

                _mapper.Map(uto, entity);

                await _db.SaveChangesAsync();
                
                return ResultModel<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"更新失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 删除供应商信息
        /// </summary>
        public async Task<ResultModel<bool>> DeleteSPAsync(int id)
        {
            try
            {
                var entity = await _db.Ask_Supplier.FindAsync(id);
                if (entity == null)
                {
                    return ResultModel<bool>.Error("记录不存在");
                }

                _db.Ask_Supplier.Remove(entity);
                await _db.SaveChangesAsync();

                return ResultModel<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"删除失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 检查供应商信息是否存在
        /// </summary>
        public async Task<ResultModel<bool>> ExistsSPAsync(string SuppName, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SuppName))
                {
                    return ResultModel<bool>.Error("供应商名称不能为空");
                }

                var query = _db.Ask_Supplier.Where(x => x.SuppName == SuppName);

                if (excludeId.HasValue)
                {
                    query = query.Where(x => x.ID != excludeId.Value);
                }

                return ResultModel<bool>.Ok(await query.AnyAsync());
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"查询失败：{ex.Message}");
            }
        }

         #endregion

        #region 供应商附件配置管理

        /// <summary>
        /// 获取供应商附件配置页面数据
        /// </summary>
        public async Task<ResultModel<List<SPFJPageDto>>> GetSPFJPageAsync(int supplierId)
        {
            try
            {
                // 使用LEFT JOIN查询：所有附件 LEFT JOIN 该供应商的供货关系
                var result = await (from fj in _db.Ask_FJList
                                   join sr in _db.Ask_SuppRangeFJ.Where(x => x.SuppID == supplierId)
                                   on fj.FJType equals sr.FJType into supplierRelations
                                   from sr in supplierRelations.DefaultIfEmpty()
                                   select new SPFJPageDto
                                   {
                                       FJType = fj.FJType!,
                                       IsSupplied = sr != null  // 有关系记录=true，否则=false
                                   }).ToListAsync();

                return ResultModel<List<SPFJPageDto>>.Ok(result);
            }
            catch (Exception ex)
            {
                return ResultModel<List<SPFJPageDto>>.Error($"查询失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 批量更新供应商附件配置
        /// </summary>
        public async Task<ResultModel<bool>> BatchUpdateSPFJAsync(int supplierId, List<string> suppliedFJTypes)
        {
            try
            {

                //  删除该供应商的所有现有配置
                var existingConfigs = await _db.Ask_SuppRangeFJ
                    .Where(x => x.SuppID == supplierId)
                    .ToListAsync();
                
                if (existingConfigs.Any())
                {
                    _db.Ask_SuppRangeFJ.RemoveRange(existingConfigs);
                }

                // 新增选中的配置
                if (suppliedFJTypes.Any())
                {
                    var newConfigs = suppliedFJTypes.Select(fjType => new Ask_SuppRangeFJ
                    {
                        SuppID = supplierId,
                        FJType = fjType
                    }).ToList();

                    await _db.Ask_SuppRangeFJ.AddRangeAsync(newConfigs);
                }

                await _db.SaveChangesAsync();
                return ResultModel<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"更新失败: {ex.Message}");
            }
        }
        #endregion

        #region 供应商阀体配置管理

        /// <summary>
        /// 获取供应商阀体配置页面数据
        /// </summary>
        public async Task<ResultModel<List<SPFTPageDto>>> GetSPFTPageAsync(int supplierId)
        {
            try
            {
                // 使用LEFT JOIN查询：所有阀体 LEFT JOIN 该供应商的供货关系
                var result = await (from ft in _db.Ask_FTList
                                   join sr in _db.Ask_SuppRangeFT.Where(x => x.SuppID == supplierId)
                                   on ft.ID equals sr.FTID into supplierRelations
                                   from sr in supplierRelations.DefaultIfEmpty()
                                   select new SPFTPageDto
                                   {
                                       FTID = ft.ID,
                                       FTName = ft.FTName ?? "",
                                       FTVersion = ft.FTVersion ?? "",
                                       IsSupplied = sr != null,  // 有关系记录=true，否则=false
                                       Lv = sr != null ? sr.lv : null
                                   }).ToListAsync();

                return ResultModel<List<SPFTPageDto>>.Ok(result);
            }
            catch (Exception ex)
            {
                return ResultModel<List<SPFTPageDto>>.Error($"查询失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 批量更新供应商阀体配置
        /// </summary>
        public async Task<ResultModel<bool>> BatchUpdateSPFTAsync(int supplierId, List<SPFTItem> suppliedFTItems, string? currentUser)
        {
            try
            {


                // 删除该供应商的所有现有配置
                var existingConfigs = await _db.Ask_SuppRangeFT
                    .Where(x => x.SuppID == supplierId)
                    .ToListAsync();
                
                if (existingConfigs.Any())
                {
                    _db.Ask_SuppRangeFT.RemoveRange(existingConfigs);
                }

                // 新增选中的配置
                if (suppliedFTItems.Any())
                {
                    var newConfigs = suppliedFTItems.Select(item => new Ask_SuppRangeFT
                    {
                        SuppID = supplierId,
                        FTID = item.FTID,
                        lv = item.Lv
                    }).ToList();

                    await _db.Ask_SuppRangeFT.AddRangeAsync(newConfigs);
                }

                await _db.SaveChangesAsync();
                return ResultModel<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"更新失败: {ex.Message}");
            }
        }

        #endregion

        #region 询价单据查询
        /// <summary>
        /// 获取询价分页数据
        /// </summary>
        public async Task<ResultModel<PaginationList<Ask_BillDto>>> GetBillPagedListAsync(Ask_BillQto qto)
        {
            try
            {
                var query = from bill in _db.Ask_Bill
                           join priceBill in _db.Price_Bill on bill.BillID equals priceBill.BillID into priceBills
                           from priceBill in priceBills.DefaultIfEmpty()
                           select new { bill, priceBill };

                // 关键字搜索：单据编号/项目名称/客户名称/发起人
                if (!string.IsNullOrWhiteSpace(qto.Keyword))
                {
                    query = query.Where(x => 
                    x.bill.BillID.ToString().Contains(qto.Keyword) || 
                    x.bill.Proj.Contains(qto.Keyword) || 
                    x.bill.ProjUser.Contains(qto.Keyword) || 
                    x.bill.KUser.Contains(qto.Keyword) ||
                    (x.priceBill.Customer != null && x.priceBill.Customer.Contains(qto.Keyword)));
                }

                if (qto.StartDate.HasValue)
                {
                    query = query.Where(x => x.bill.KDate >= qto.StartDate.Value);
                }

                if (qto.EndDate.HasValue)
                {
                    var endDate = qto.EndDate.Value.AddDays(1);
                    query = query.Where(x => x.bill.KDate < endDate);
                }

                query = query.OrderBy(x => x.bill.BillID);

                var projectedQuery = query.Select(x => new Ask_BillDto
                {
                    BillID = x.bill.BillID,
                    Proj = x.bill.Proj,
                    ProjUser = x.bill.ProjUser,
                    KUser = x.bill.KUser,
                    BillState = x.bill.BillState,
                    KDate = x.bill.KDate,
                    Customer = x.priceBill.Customer
                });

                var result = await PaginationList<Ask_BillDto>.CreateAsync(qto.PageNumber, qto.PageSize, projectedQuery);
                return ResultModel<PaginationList<Ask_BillDto>>.Ok(result);
            }
            catch (Exception ex)
            {
                return ResultModel<PaginationList<Ask_BillDto>>.Error($"查询失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 询价订单详情
        /// <summary>
        public async Task<ResultModel<List<Ask_BillDetailDto>>> GetBillDetailsAsync(string billId)
        {
            try
            {
                // 使用 LEFT JOIN 查询：所有明细 LEFT JOIN 对应的价格信息
                var query = from detail in _db.Ask_BillDetail.Where(d => d.BillID == int.Parse(billId))

                            join priceInfo in _db.Ask_BillPrice
                            on detail.ID equals priceInfo.BillDetailID into gj 

                            from price in gj.DefaultIfEmpty() 

                            orderby detail.ID

                            select new Ask_BillDetailDto
                            {
                                Type = detail.Type,
                                Version = detail.Version,
                                Name = detail.Name,
                                DN = detail.DN,
                                PN = detail.PN,
                                LJ = detail.LJ,
                                FG = detail.FG,
                                FT = detail.FT,
                                FNJ = detail.FNJ,
                                ordMed = detail.ordMed,
                                OrdKV = detail.OrdKV,
                                ordFW = detail.ordFW,
                                ordLeak = detail.ordLeak,
                                ordQY = detail.ordQY,
                                TL = detail.TL,
                                State = detail.State,
                                Memo = detail.Memo,
                                CGPriceMemo = detail.CGPriceMemo,

                                Price = (price != null) ? price.Price : null,
                                KDate = (price != null) ? price.KDate : null,
                                Remarks = (price != null) ? price.Remarks : null
                            };

                var result = await query.ToListAsync();

                return ResultModel<List<Ask_BillDetailDto>>.Ok(result);
            }
            catch (Exception ex)
            {
                return ResultModel<List<Ask_BillDetailDto>>.Error($"查询明细失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取询价的状态日志
        /// </summary>
        public async Task<ResultModel<List<Ask_BillLogDto>>> GetBillLogsAsync(Ask_BillLogQto qto)
        {
            try
            {
                var logs = await (from log in _db.Ask_BillLog.Where(x => x.BillDetailID == qto.BillDetailID)
                                  join price in _db.Ask_BillPrice on log.BillDetailID equals price.BillDetailID into prices
                                  from price in prices.DefaultIfEmpty()
                                  orderby log.KDate
                                  select new Ask_BillLogDto
                                  {
                                      KUser = log.KUser,
                                      KDate = log.KDate,
                                      State = ZKLT25Profile.GetBillStateText(log.State),
                                      Price = price != null ? price.Price : null,
                                      Remarks = price != null ? price.Remarks : null
                                  }).ToListAsync();

                return ResultModel<List<Ask_BillLogDto>>.Ok(logs);
            }
            catch (Exception ex)
            {
                return ResultModel<List<Ask_BillLogDto>>.Error($"查询失败: {ex.Message}");
            }
        }


        #endregion
    }
}