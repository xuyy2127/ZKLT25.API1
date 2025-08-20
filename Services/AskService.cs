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
using Aspose.Cells;

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

        #region 阀体附件价格查询
        /// <summary>
        /// 分页查阀体价格
        /// </summary>
        public async Task<ResultModel<PaginationList<Ask_DataFTDto>>> GetDataFTPagedListAsync(Ask_DataFTQto qto)
        {
            try
            {
                // 分别构建内部和外部来源的查询，再按需 Union
                var internalQueryFT = from d in _db.Ask_DataFT
                                      join supplier in _db.Ask_Supplier on d.Supplier equals supplier.ID.ToString() into supplierGroup
                                      from supplier in supplierGroup.DefaultIfEmpty()
                                      join billPrice in _db.Ask_BillPrice on d.BillDetailID equals billPrice.BillDetailID into priceGroup
                                      from billPrice in priceGroup.DefaultIfEmpty()
                                      join billFile in _db.Ask_BillFile on d.BillDetailID equals billFile.BillDetailID into fileGroup
                                      from billFile in fileGroup.DefaultIfEmpty()
                                      join billDetail in _db.Ask_BillDetail on d.BillDetailID equals billDetail.ID into detailGroup
                                      from billDetail in detailGroup.DefaultIfEmpty()
                                      join bill in _db.Ask_Bill on billDetail.BillID equals bill.BillID into billGroup
                                      from bill in billGroup.DefaultIfEmpty()
                                      select new Ask_DataFTDto
                                      {
                                          ID = d.ID,
                                          BillID = billDetail != null ? billDetail.BillID : null,
                                          AskProjName = d.AskProjName,
                                          SuppName = supplier != null ? supplier.SuppName : "",
                                          Price = billPrice != null ? billPrice.Price : null,
                                          BasicsPrice = billPrice != null ? billPrice.BasicsPrice : null,
                                          AddPrice = billPrice != null ? billPrice.AddPrice : null,
                                          AskDate = d.AskDate,
                                          OrdName = d.OrdName,
                                          OrdVersion = d.OrdVersion,
                                          OrdDN = d.OrdDN,
                                          OrdPN = d.OrdPN,
                                          OrdLJ = d.OrdLJ,
                                          OrdFG = d.OrdFG,
                                          OrdFT = d.OrdFT,
                                          OrdFNJ = d.OrdFNJ,
                                          OrdTL = d.OrdTL,
                                          OrdKV = d.OrdKV,
                                          OrdFlow = d.OrdFlow,
                                          OrdLeak = d.OrdLeak,
                                          OrdQYDY = d.OrdQYDY,
                                          Num = d.Num,
                                          OrdUnit = d.OrdUnit,
                                          IsPreProBind = d.IsPreProBind,
                                          DocName = bill != null ? bill.DocName : null,
                                          DocNameStatus = !string.IsNullOrWhiteSpace(bill != null ? bill.DocName : null) ? "已上传" : "未上传",
                                          FileName = billFile != null ? billFile.FileName : null,
                                          FileNameStatus = !string.IsNullOrWhiteSpace(billFile != null ? billFile.FileName : null) ? "已上传" : "未上传",
                                          IsInvalid = 1,
                                          Timeout = d.Timeout
                                      };

                var externalQueryFT = from d in _db.Set<Ask_DataFTOut>()
                                      join supplier in _db.Ask_Supplier on d.Supplier equals supplier.ID.ToString() into supplierGroup
                                      from supplier in supplierGroup.DefaultIfEmpty()
                                      join billPrice in _db.Ask_BillPrice on d.BillDetailID equals billPrice.BillDetailID into priceGroup
                                      from billPrice in priceGroup.DefaultIfEmpty()
                                      join billFile in _db.Ask_BillFile on d.BillDetailID equals billFile.BillDetailID into fileGroup
                                      from billFile in fileGroup.DefaultIfEmpty()
                                      join billDetail in _db.Ask_BillDetail on d.BillDetailID equals billDetail.ID into detailGroup
                                      from billDetail in detailGroup.DefaultIfEmpty()
                                      join bill in _db.Ask_Bill on billDetail.BillID equals bill.BillID into billGroup
                                      from bill in billGroup.DefaultIfEmpty()
                                      select new Ask_DataFTDto
                                      {
                                          ID = d.ID,
                                          BillID = billDetail != null ? billDetail.BillID : null,
                                          AskProjName = d.AskProjName,
                                          SuppName = supplier != null ? supplier.SuppName : "",
                                          Price = billPrice != null ? billPrice.Price : null,
                                          BasicsPrice = billPrice != null ? billPrice.BasicsPrice : null,
                                          AddPrice = billPrice != null ? billPrice.AddPrice : null,
                                          AskDate = d.AskDate,
                                          OrdName = d.OrdName,
                                          OrdVersion = d.OrdVersion,
                                          OrdDN = d.OrdDN,
                                          OrdPN = d.OrdPN,
                                          OrdLJ = d.OrdLJ,
                                          OrdFG = d.OrdFG,
                                          OrdFT = d.OrdFT,
                                          OrdFNJ = d.OrdFNJ,
                                          OrdTL = d.OrdTL,
                                          OrdKV = d.OrdKV,
                                          OrdFlow = d.OrdFlow,
                                          OrdLeak = d.OrdLeak,
                                          OrdQYDY = d.OrdQYDY,
                                          Num = d.Num,
                                          OrdUnit = d.OrdUnit,
                                          IsPreProBind = d.IsPreProBind,
                                          DocName = bill != null ? bill.DocName : null,
                                          DocNameStatus = !string.IsNullOrWhiteSpace(bill != null ? bill.DocName : null) ? "已上传" : "未上传",
                                          FileName = billFile != null ? billFile.FileName : null,
                                          FileNameStatus = !string.IsNullOrWhiteSpace(billFile != null ? billFile.FileName : null) ? "已上传" : "未上传",
                                          IsInvalid = 0,
                                          Timeout = d.Timeout
                                      };

                IQueryable<Ask_DataFTDto> query;
                if (qto.IsOutView == 1)
                {
                    query = internalQueryFT;
                }
                else if (qto.IsOutView == 0)
                {
                    query = externalQueryFT;
                }
                else
                {
                    query = internalQueryFT.Union(externalQueryFT);
                }

                if (qto.BillID.HasValue)
                {
                    query = query.Where(x => x.BillID == qto.BillID.Value);
                }

                if (!string.IsNullOrWhiteSpace(qto.AskProjName))
                {
                    query = query.Where(x => x.AskProjName.Contains(qto.AskProjName));
                }

                if (!string.IsNullOrWhiteSpace(qto.SuppName))
                {
                    query = query.Where(x => x.SuppName.Contains(qto.SuppName));
                }

                if (!string.IsNullOrWhiteSpace(qto.OrdVersion))
                {
                    query = query.Where(x => x.OrdVersion.Contains(qto.OrdVersion));
                }

                if (!string.IsNullOrWhiteSpace(qto.OrdDN))
                {
                    query = query.Where(x => x.OrdDN.Contains(qto.OrdDN));
                }

                if (!string.IsNullOrWhiteSpace(qto.OrdPN))
                {
                    query = query.Where(x => x.OrdPN.Contains(qto.OrdPN));
                }

                if (!string.IsNullOrWhiteSpace(qto.OrdFT))
                {
                    query = query.Where(x => x.OrdFT.Contains(qto.OrdFT));
                }

                // 日期范围查询
                if (qto.StartDate.HasValue)
                {
                    query = query.Where(x => x.AskDate >= qto.StartDate.Value);
                }

                if (qto.EndDate.HasValue)
                {
                    query = query.Where(x => x.AskDate <= qto.EndDate.Value);
                }

                // 价格状态过滤
                if (qto.IsExpired.HasValue)
                {
                    if (qto.IsExpired.Value)
                    {
                        // 已过期：Timeout > 0
                        query = query.Where(x => x.Timeout > 0);
                    }
                    else
                    {
                        // 未过期：Timeout <= 0
                        query = query.Where(x => x.Timeout <= 0);
                    }
                }

                // 排序
                query = query.OrderByDescending(x => x.AskDate);

                // 分页
                var pagedResult = await PaginationList<Ask_DataFTDto>.CreateAsync(qto.PageNumber, qto.PageSize, query);

                foreach (var item in pagedResult)
                {
                    item.IsPreProBindText = ZKLT25Profile.GetPreProBindText(item.IsPreProBind);
                    item.PriceStatusText = ZKLT25Profile.GetPriceStatusText(item.Timeout);
                    item.AvailableActions = ZKLT25Profile.GetAvailableActions(item.Timeout);
                }
                
                return ResultModel<PaginationList<Ask_DataFTDto>>.Ok(pagedResult);
            }
            catch (Exception ex)
            {
                return ResultModel<PaginationList<Ask_DataFTDto>>.Error($"查询失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 分页查询附件价格
        /// </summary>
        public async Task<ResultModel<PaginationList<Ask_DataFJDto>>> GetDataFJPagedListAsync(Ask_DataFJQto qto)
        {
            try
            {
                var internalQueryFJ = from d in _db.Ask_DataFJ
                                      join billDetail in _db.Ask_BillDetail on d.BillDetailID equals billDetail.ID
                                      join bill in _db.Ask_Bill on billDetail.BillID equals bill.BillID
                                      join supplier in _db.Ask_Supplier on d.Supplier equals supplier.ID.ToString() into supplierGroup
                                      from supplier in supplierGroup.DefaultIfEmpty()
                                      join bp in (
                                          from p in _db.Ask_BillPrice
                                          group p by p.BillDetailID into g
                                          select new { BillDetailID = g.Key, BasicsPrice = g.Max(x => x.BasicsPrice ?? 0), AddPrice = g.Max(x => x.AddPrice ?? 0) }
                                      ) on d.BillDetailID equals bp.BillDetailID into priceGroup
                                      from billPrice in priceGroup.DefaultIfEmpty()
                                      join billFile in _db.Ask_BillFile on d.BillDetailID equals billFile.BillDetailID into fileGroup
                                      from billFile in fileGroup.DefaultIfEmpty()
                                      select new Ask_DataFJDto
                                      {
                                          ID = d.ID,
                                          BillID = billDetail.BillID,
                                          AskProjName = d.AskProjName,
                                          SuppName = supplier != null ? supplier.SuppName : "",
                                          Price = (double?)d.Price,
                                          BasicsPrice = billPrice != null ? billPrice.BasicsPrice : null,
                                          AddPrice = billPrice != null ? billPrice.AddPrice : null,
                                          AskDate = d.AskDate,
                                          OrdName = d.FJType,
                                          FJVersion = d.FJVersion,
                                          Num = d.Num,
                                          Unit = d.Unit,
                                          IsPreProBind = d.IsPreProBind,
                                          DocName = bill.DocName,
                                          FileName = billFile != null ? billFile.FileName : null,
                                          Timeout = d.Timeout, 
                                          DocNameStatus = !string.IsNullOrWhiteSpace(bill.DocName) ? "已上传" : "未上传",
                                          FileNameStatus = !string.IsNullOrWhiteSpace(billFile != null ? billFile.FileName : null) ? "已上传" : "未上传",
                                          IsInvalid = 1,
                                          ProjDay = d.ProjDay,
                                          Day1 = d.Day1,
                                          Day2 = d.Day2,
                                          Memo1 = d.Memo1,
                                          BillIDText = billDetail.BillID.ToString()
                                      };

                var externalQueryFJ = from d in _db.Ask_DataFJOut
                                      join billDetail in _db.Ask_BillDetail on d.BillDetailID equals billDetail.ID
                                      join bill in _db.Ask_Bill on billDetail.BillID equals bill.BillID
                                      join supplier in _db.Ask_Supplier on d.Supplier equals supplier.ID.ToString() into supplierGroup
                                      from supplier in supplierGroup.DefaultIfEmpty()
                                      join bp in (
                                          from p in _db.Ask_BillPrice
                                          group p by p.BillDetailID into g
                                          select new { BillDetailID = g.Key, BasicsPrice = g.Max(x => x.BasicsPrice ?? 0), AddPrice = g.Max(x => x.AddPrice ?? 0) }
                                      ) on d.BillDetailID equals bp.BillDetailID into priceGroup
                                      from billPrice in priceGroup.DefaultIfEmpty()
                                      join billFile in _db.Ask_BillFile on d.BillDetailID equals billFile.BillDetailID into fileGroup
                                      from billFile in fileGroup.DefaultIfEmpty()
                                      select new Ask_DataFJDto
                                      {
                                          ID = d.ID,
                                          BillID = billDetail.BillID,
                                          AskProjName = d.AskProjName,
                                          SuppName = supplier != null ? supplier.SuppName : "",
                                          Price = (double?)d.Price,
                                          BasicsPrice = billPrice != null ? billPrice.BasicsPrice : null,
                                          AddPrice = billPrice != null ? billPrice.AddPrice : null,
                                          AskDate = d.AskDate,
                                          OrdName = d.FJType,
                                          FJVersion = d.FJVersion,
                                          Num = d.Num,
                                          Unit = d.Unit,
                                          IsPreProBind = d.IsPreProBind,
                                          DocName = bill.DocName,
                                          FileName = billFile != null ? billFile.FileName : null,
                                          Timeout = d.Timeout,
                                          DocNameStatus = !string.IsNullOrWhiteSpace(bill.DocName) ? "已上传" : "未上传",
                                          FileNameStatus = !string.IsNullOrWhiteSpace(billFile != null ? billFile.FileName : null) ? "已上传" : "未上传",
                                          IsInvalid = 0,
                                          ProjDay = d.ProjDay,
                                          Day1 = d.Day1,
                                          Day2 = d.Day2,
                                          Memo1 = d.Memo1,
                                          BillIDText = billDetail.BillID.ToString()
                                      };

                IQueryable<Ask_DataFJDto> query;
                if (qto.IsOutView == 1)
                {
                    query = internalQueryFJ;
                }
                else if (qto.IsOutView == 0)
                {
                    query = externalQueryFJ;
                }
                else
                {
                    query = internalQueryFJ.Union(externalQueryFJ);
                }

                // 条件筛选
                if (qto.BillID.HasValue)
                {
                    query = query.Where(x => x.BillID == qto.BillID.Value);
                }

                if (!string.IsNullOrWhiteSpace(qto.AskProjName))
                {
                    query = query.Where(x => x.AskProjName.Contains(qto.AskProjName));
                }

                if (!string.IsNullOrWhiteSpace(qto.SuppName))
                {
                    query = query.Where(x => x.SuppName.Contains(qto.SuppName));
                }

                if (!string.IsNullOrWhiteSpace(qto.OrdName))
                {
                    query = query.Where(x => x.OrdName.Contains(qto.OrdName)); 
                }

                if (!string.IsNullOrWhiteSpace(qto.FJVersion))
                {
                    query = query.Where(x => x.FJVersion.Contains(qto.FJVersion));
                }

                // 日期范围查询
                if (qto.StartDate.HasValue)
                {
                    query = query.Where(x => x.AskDate >= qto.StartDate.Value);
                }

                if (qto.EndDate.HasValue)
                {
                    query = query.Where(x => x.AskDate <= qto.EndDate.Value);
                }

                // 价格状态过滤
                if (qto.IsExpired.HasValue)
                {
                    if (qto.IsExpired.Value)
                    {
                        // 已过期：Timeout > 0
                        query = query.Where(x => x.Timeout > 0);
                    }
                    else
                    {
                        // 未过期：Timeout <= 0
                        query = query.Where(x => x.Timeout <= 0);
                    }
                }

                // 排序
                query = query.OrderByDescending(x => x.AskDate);

                // 分页
                var pagedResult = await PaginationList<Ask_DataFJDto>.CreateAsync(qto.PageNumber, qto.PageSize, query);
                
                foreach (var item in pagedResult)
                {
                    item.IsPreProBindText = ZKLT25Profile.GetPreProBindText(item.IsPreProBind);
                    item.PriceStatusText = ZKLT25Profile.GetPriceStatusText(item.Timeout);
                    item.AvailableActions = ZKLT25Profile.GetAvailableActions(item.Timeout);
                    item.DocNameStatus = string.IsNullOrWhiteSpace(item.DocName) ? "未上传" : "已上传";
                    item.FileNameStatus = string.IsNullOrWhiteSpace(item.FileName) ? "未上传" : "已上传";
                }
                
                return ResultModel<PaginationList<Ask_DataFJDto>>.Ok(pagedResult);
            }
            catch (Exception ex)
            {
                return ResultModel<PaginationList<Ask_DataFJDto>>.Error($"查询失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 设置价格状态
        /// </summary>
        public async Task<ResultModel<bool>> SetPriceStatusAsync(List<int> ids, string action, int? extendDays, string? currentUser, string entityType)
        {
            try
            {
                var newTimeoutValue = CalculateTimeoutValue(action, extendDays);
                if (!newTimeoutValue.HasValue)
                {
                    return ResultModel<bool>.Error("延长有效期需要指定有效的天数");
                }

                switch (entityType.ToUpper())
                {
                    case "DATAFT":
                        {
                            var entities = await _db.Ask_DataFT.Where(x => ids.Contains(x.ID)).ToListAsync();
                            foreach (var entity in entities)
                            {
                                entity.Timeout = newTimeoutValue.Value;
                            }
                        }
                        break;
                    case "DATAFJ":
                        {
                            var entities = await _db.Ask_DataFJ.Where(x => ids.Contains(x.ID)).ToListAsync();
                            foreach (var entity in entities)
                            {
                                entity.Timeout = newTimeoutValue.Value;
                            }
                        }
                        break;
                    default:
                        return ResultModel<bool>.Error("无效的实体类型");
                }

                await _db.SaveChangesAsync();
                return ResultModel<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"操作失败：{ex.Message}");
            }
        }



        /// <summary>
        /// 提取状态计算逻辑
        /// </summary>
        private int? CalculateTimeoutValue(string action, int? extendDays)
        {
            switch (action.ToUpper())
            {
                case "SETVALID":
                    return -1;
                case "SETEXPIRED":
                    return 1;
                case "EXTENDVALID":
                    if (!extendDays.HasValue || extendDays.Value <= 0)
                        return null;
                    return -extendDays.Value;
                default:
                    throw new ArgumentException("无效的操作类型");
            }
        }
        #endregion

        #region 导出为excel表
        /// <summary>
        /// 导出附件询价数据为 Excel 文件
        /// </summary>
        public async Task<byte[]> DataFJExcelAsync(Ask_DataFJQto qto)
        {
            try
            {
                var result = await GetDataFJPagedListAsync(new Ask_DataFJQto
                {
                    BillID = qto.BillID,
                    AskProjName = qto.AskProjName,
                    SuppName = qto.SuppName,
                    OrdName = qto.OrdName,
                    FJVersion = qto.FJVersion,
                    StartDate = qto.StartDate,
                    EndDate = qto.EndDate,
                    IsOutView = qto.IsOutView,
                    IsExpired = qto.IsExpired,
                    PageSize = 999999, // 取全量数据
                    PageNumber = 1
                });

                if (!result.Success || result.Data == null)
                {
                    throw new Exception(result.Message ?? "获取数据失败");
                }

                var exportData = result.Data;

                var workbook = new Workbook();
                var worksheet = workbook.Worksheets[0];
                worksheet.Name = "附件询价数据";

                var headers = new[] {
                    "询价日期", "单据编号", "项目名称", "物品名称", "物品型号", "单位", "数量",
                    "成本价", "供应商名称", "项目交期(天)", "小于10台标准交期(天)", "10台至20台标准交期(天)",
                    "备注1", "报价文件", "明细表"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = worksheet.Cells[0, i];
                    cell.Value = headers[i];
                    var style = cell.GetStyle();
                    style.Font.IsBold = true;
                    style.HorizontalAlignment = TextAlignmentType.Center;
                    cell.SetStyle(style);
                }

                for (int row = 0; row < exportData.Count; row++)
                {
                    var data = exportData[row];
                    worksheet.Cells[row + 1, 0].Value = data.AskDate?.ToString("yyyy-MM-dd") ?? "";
                    worksheet.Cells[row + 1, 1].Value = data.BillIDText ?? "";
                    worksheet.Cells[row + 1, 2].Value = data.AskProjName ?? "";
                    worksheet.Cells[row + 1, 3].Value = data.OrdName ?? "";
                    worksheet.Cells[row + 1, 4].Value = data.FJVersion ?? "";
                    worksheet.Cells[row + 1, 5].Value = data.Unit ?? "";
                    worksheet.Cells[row + 1, 6].Value = data.Num?.ToString() ?? "";
                    worksheet.Cells[row + 1, 7].Value = data.Price?.ToString("F2") ?? "";
                    worksheet.Cells[row + 1, 8].Value = data.SuppName ?? "";
                    worksheet.Cells[row + 1, 9].Value = data.ProjDay ?? "";
                    worksheet.Cells[row + 1, 10].Value = data.Day1 ?? "";
                    worksheet.Cells[row + 1, 11].Value = data.Day2 ?? "";
                    worksheet.Cells[row + 1, 12].Value = data.Memo1 ?? "";
                    worksheet.Cells[row + 1, 13].Value = data.FileNameStatus;
                    worksheet.Cells[row + 1, 14].Value = data.DocNameStatus;
                }

                for (int i = 0; i < Math.Min(headers.Length, 8); i++)
                {
                    worksheet.AutoFitColumn(i);
                }

                using var stream = new MemoryStream();
                workbook.Save(stream, SaveFormat.Xlsx);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception($"导出失败：{ex.Message}", ex);
            }
        }

        #endregion

        #region 价格/备注录入

        /// <summary>
        /// 录入价格备注
        /// </summary>
        /// <param name="cto">价格录入请求</param>
        /// <param name="currentUser">当前用户</param>
        /// <returns>返回影响的记录数</returns>
        public async Task<ResultModel<int>> SetPriceRemarkAsync(BillPriceCto cto, string? currentUser)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var targetDetails = await _db.Ask_BillDetail
                    .Where(x => cto.BillDetailIDs.Contains(x.ID) && x.State == 0)
                    .ToListAsync();

                // 批量操作验证Type和Version是否一致
                if (cto.BillDetailIDs.Count > 1)
                {
                    var firstRecord = targetDetails.First();
                    var differentRecords = targetDetails
                        .Where(x => x.Type != firstRecord.Type || x.Version != firstRecord.Version)
                        .ToList();
                    
                    if (differentRecords.Any())
                    {
                        return ResultModel<int>.Error($"类型和型号必须相同。首个记录：{firstRecord.Type}-{firstRecord.Version}");
                    }
                }

                var allDetailIDs = targetDetails.Select(d => d.ID).ToList();
                var supplierIdStr = cto.SuppID?.ToString();
                var timeout = -(cto.ValidityDays ?? 1);
                var isPreProBind = cto.IsPreProBind ?? 0;

                var existingBillPrices = await _db.Ask_BillPrice
                    .Where(p => allDetailIDs.Contains(p.BillDetailID.Value) && p.SuppID == cto.SuppID)
                    .ToListAsync();
                var billPriceDict = existingBillPrices.ToDictionary(x => x.BillDetailID.Value);

                var valveBodyIds = targetDetails.Where(x => x.Type == "阀体").Select(x => x.ID).ToList();
                var attachmentIds = targetDetails.Where(x => x.Type != "阀体").Select(x => x.ID).ToList();

                var dataFTList = valveBodyIds.Any() 
                    ? await _db.Ask_DataFT.Where(x => valveBodyIds.Contains(x.BillDetailID.Value) && x.Supplier == supplierIdStr).ToListAsync()
                    : new List<Ask_DataFT>();
                var dataFTOutList = valveBodyIds.Any()
                    ? await _db.Ask_DataFTOut.Where(x => valveBodyIds.Contains(x.BillDetailID.Value) && x.Supplier == supplierIdStr).ToListAsync()
                    : new List<Ask_DataFTOut>();

                var dataFJList = attachmentIds.Any()
                    ? await _db.Ask_DataFJ.Where(x => attachmentIds.Contains(x.BillDetailID.Value) && x.Supplier == supplierIdStr).ToListAsync()
                    : new List<Ask_DataFJ>();
                var dataFJOutList = attachmentIds.Any()
                    ? await _db.Ask_DataFJOut.Where(x => attachmentIds.Contains(x.BillDetailID.Value) && x.Supplier == supplierIdStr).ToListAsync()
                    : new List<Ask_DataFJOut>();

                foreach (var detail in targetDetails)
                {
                    // 创建/更新 BillPrice
                    if (!billPriceDict.TryGetValue(detail.ID, out var billPrice))
                    {
                        billPrice = new Ask_BillPrice
                        {
                            BillDetailID = detail.ID,
                            SuppID = cto.SuppID,
                            KDate = DateTime.Now,
                            KUser = currentUser
                        };
                        _db.Ask_BillPrice.Add(billPrice);
                        billPriceDict[detail.ID] = billPrice;
                    }
                    
                    // 更新价格字段
                    billPrice.Price = cto.Price;
                    billPrice.Num = cto.Num;
                    billPrice.BasicsPrice = cto.BasicsPrice;
                    billPrice.AddPrice = cto.AddPrice;
                    billPrice.Remarks = cto.Remarks;
                    billPrice.KDate = DateTime.Now;
                    billPrice.KUser = currentUser;

                    // 更新BillDetail备注和状态
                    if (!string.IsNullOrWhiteSpace(cto.CGPriceMemo))
                    {
                        detail.CGPriceMemo = cto.CGPriceMemo;
                    }
                    detail.State = 2; // 已完成

                    // 写入日志
                    var billLog = new Ask_BillLog
                    {
                        BillDetailID = detail.ID,
                        State = 2,
                        KDate = DateTime.Now,
                        KUser = currentUser
                    };
                    _db.Ask_BillLog.Add(billLog);
                }

                // 批量更新
                foreach (var item in dataFTList)
                {
                    item.Timeout = timeout;
                    item.IsPreProBind = isPreProBind;
                }
                foreach (var item in dataFTOutList)
                {
                    item.Timeout = timeout;
                    item.IsPreProBind = isPreProBind;
                }
                foreach (var item in dataFJList)
                {
                    item.Timeout = timeout;
                    item.IsPreProBind = isPreProBind;
                }
                foreach (var item in dataFJOutList)
                {
                    item.Timeout = timeout;
                    item.IsPreProBind = isPreProBind;
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return ResultModel<int>.Ok(targetDetails.Count);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ResultModel<int>.Error($"操作失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 关闭项目（将状态从发起0改为已关闭-1）
        /// </summary>
        /// <param name="billDetailIds">要关闭的明细ID列表</param>
        /// <param name="currentUser">当前用户</param>
        /// <returns>操作结果</returns>
        public async Task<ResultModel<int>> CloseProjectAsync(List<int> billDetailIds, string? currentUser)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var targetDetails = await _db.Ask_BillDetail
                    .Where(x => billDetailIds.Contains(x.ID) && x.State == 0)
                    .ToListAsync();

                foreach (var detail in targetDetails)
                {
                    detail.State = -1; 
                    
                    var billLog = new Ask_BillLog
                    {
                        BillDetailID = detail.ID,
                        State = -1,
                        KDate = DateTime.Now,
                        KUser = currentUser
                    };
                    _db.Ask_BillLog.Add(billLog);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return ResultModel<int>.Ok(targetDetails.Count);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ResultModel<int>.Error($"关闭项目失败：{ex.Message}");
            }
        }


        #endregion


    }
}