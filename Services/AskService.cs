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


        #region 日志记录
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

        /// <summary>
        /// 根据文件名返回上传状态文本
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>下载/未上传</returns>
        private static string GetUploadStatusText(string? fileName)
        {
            return string.IsNullOrWhiteSpace(fileName) ? "未上传" : "下载";
        }
        #endregion

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
                           join deliveryBill in _db.AskDay_Bill on bill.BillID equals deliveryBill.PriceBillID into deliveryBills
                           from deliveryBill in deliveryBills.DefaultIfEmpty()
                           select new { bill, priceBill, deliveryBill };

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
                    YSDate = x.bill.YSDate,
                    Customer = x.priceBill.Customer,
                    DeliveryBillID = x.deliveryBill.BillID
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
                IQueryable<Ask_DataFTDto> query;
                
                // 根据过期状态筛选确定查询哪张表
                if (qto.IsExpired.HasValue)
                {
                    if (qto.IsExpired.Value)
                    {
                        query = BuildDataFTExpiredQuery();
                    }
                    else
                    {
                        query = BuildDataFTActiveQuery();
                    }
                }
                else
                {
                    // 显示全部数据：合并两张表
                    var activeQuery = BuildDataFTActiveQuery();
                    var expiredQuery = BuildDataFTExpiredQuery();
                    query = activeQuery.Union(expiredQuery);
                }

                // 应用其他筛选条件
                query = ApplyFTFilters(query, qto, false);
                query = ApplySorting(query);

                // 分页
                var pagedResult = await PaginateAsync(qto.PageNumber, qto.PageSize, query);

                // 装饰派生显示字段
                DecorateDataList(pagedResult);
                
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
                // 根据过期状态筛选确定查询哪张表
                IQueryable<Ask_DataFJDto> query;
                
                if (qto.IsExpired.HasValue)
                {
                    if (qto.IsExpired.Value)
                    {
                        query = BuildDataFJExpiredQuery();
                    }
                    else
                    {
                        query = BuildDataFJActiveQuery();
                    }
                }
                else
                {
                    // 显示全部数据：合并两张表
                    var activeQuery = BuildDataFJActiveQuery();
                    var expiredQuery = BuildDataFJExpiredQuery();
                    query = activeQuery.Union(expiredQuery);
                }

                // 应用其他筛选条件
                query = ApplyFJFilters(query, qto, false);
                query = ApplySorting(query);

                // 分页
                var pagedResult = await PaginateAsync(qto.PageNumber, qto.PageSize, query);
                
                // 装饰派生显示字段
                DecorateDataList(pagedResult);
                
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
                return await HandleDataStatusChangeAsync(ids, action, extendDays, currentUser, entityType);
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"操作失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 通用的数据状态变更处理
        /// </summary>
        private async Task<ResultModel<bool>> HandleDataStatusChangeAsync(List<int> ids, string action, int? extendDays, string? currentUser, string entityType)
        {
            switch (action.ToUpper())
            {
                case "SETVALID":
                    // 设置有效
                    return await MigrateDataAsync(ids, fromExpired: true, currentUser, entityType);
                case "SETEXPIRED":
                    // 设置过期
                    return await MigrateDataAsync(ids, fromExpired: false, currentUser, entityType);
                case "EXTENDVALID":
                    // 延长有效期
                    if (!extendDays.HasValue || extendDays.Value <= 0)
                {
                    return ResultModel<bool>.Error("延长有效期需要指定有效的天数");
                    }
                    return await ExtendDataTimeoutAsync(ids, extendDays.Value, currentUser, entityType);
                default:
                    return ResultModel<bool>.Error("无效的操作类型");
            }
                }

        /// <summary>
        /// 通用数据迁移调度方法
        /// </summary>
        private async Task<ResultModel<bool>> MigrateDataAsync(List<int> ids, bool fromExpired, string? currentUser, string entityType)
        {
                switch (entityType.ToUpper())
                {
                    case "DATAFT":
                    return await MigrateDataFTAsync(ids, fromExpired, currentUser);
                case "DATAFJ":
                    return await MigrateDataFJAsync(ids, fromExpired, currentUser);
                default:
                    return ResultModel<bool>.Error("无效的实体类型");
            }
        }

        /// <summary>
        /// 迁移阀体数据
        /// </summary>
        private async Task<ResultModel<bool>> MigrateDataFTAsync(List<int> ids, bool fromExpired, string? currentUser)
        {
            try
            {
                if (fromExpired)
                {
                    // 从已过期表迁移到未过期表
                    var outEntities = await _db.Ask_DataFTOut.Where(x => ids.Contains(x.ID)).ToListAsync();
                    

                    var newEntities = outEntities.Select(outEntity => new Ask_DataFT
                    {
                        Source = outEntity.Source,
                        AskDate = outEntity.AskDate,
                        AskProjName = outEntity.AskProjName,
                        OrdNum = outEntity.OrdNum,
                        OrdMed = outEntity.OrdMed,
                        OrdName = outEntity.OrdName,
                        OrdVersion = outEntity.OrdVersion,
                        OrdDN = outEntity.OrdDN,
                        OrdPN = outEntity.OrdPN,
                        OrdLJ = outEntity.OrdLJ,
                        OrdFG = outEntity.OrdFG,
                        OrdFT = outEntity.OrdFT,
                        OrdFNJ = outEntity.OrdFNJ,
                        OrdTL = outEntity.OrdTL,
                        OrdKV = outEntity.OrdKV,
                        OrdFlow = outEntity.OrdFlow,
                        OrdLeak = outEntity.OrdLeak,
                        OrdQYDY = outEntity.OrdQYDY,
                        OrdUnit = outEntity.OrdUnit,
                        Num = outEntity.Num,
                        Memo = outEntity.Memo,
                        AskRequire = outEntity.AskRequire,
                        Price = outEntity.Price,
                        Supplier = outEntity.Supplier,
                        ProjDay = outEntity.ProjDay,
                        Day1 = outEntity.Day1,
                        Day2 = outEntity.Day2,
                        Day3 = outEntity.Day3,
                        Memo1 = outEntity.Memo1,
                        DoUser = currentUser,
                        DoDate = DateTime.Now,
                        PriceRatio = outEntity.PriceRatio,
                        BillDetailID = outEntity.BillDetailID,
                        IsPreProBind = outEntity.IsPreProBind,
                        Timeout = -1 // 设置为有效
                    }).ToList();

                    _db.Ask_DataFT.AddRange(newEntities);
                    _db.Ask_DataFTOut.RemoveRange(outEntities);
                }
                else
                {
                    // 从未过期表迁移到已过期表
                            var entities = await _db.Ask_DataFT.Where(x => ids.Contains(x.ID)).ToListAsync();

                    var outEntities = entities.Select(entity => new Ask_DataFTOut
                    {
                        Source = entity.Source,
                        AskDate = entity.AskDate,
                        AskProjName = entity.AskProjName,
                        OrdNum = entity.OrdNum,
                        OrdMed = entity.OrdMed,
                        OrdName = entity.OrdName,
                        OrdVersion = entity.OrdVersion,
                        OrdDN = entity.OrdDN,
                        OrdPN = entity.OrdPN,
                        OrdLJ = entity.OrdLJ,
                        OrdFG = entity.OrdFG,
                        OrdFT = entity.OrdFT,
                        OrdFNJ = entity.OrdFNJ,
                        OrdTL = entity.OrdTL,
                        OrdKV = entity.OrdKV,
                        OrdFlow = entity.OrdFlow,
                        OrdLeak = entity.OrdLeak,
                        OrdQYDY = entity.OrdQYDY,
                        OrdUnit = entity.OrdUnit,
                        Num = entity.Num,
                        Memo = entity.Memo,
                        AskRequire = entity.AskRequire,
                        Price = (float?)entity.Price,
                        Supplier = entity.Supplier,
                        ProjDay = entity.ProjDay,
                        Day1 = entity.Day1,
                        Day2 = entity.Day2,
                        Day3 = entity.Day3,
                        Memo1 = entity.Memo1,
                        DoUser = currentUser,
                        DoDate = DateTime.Now,
                        PriceRatio = (float?)entity.PriceRatio,
                        BillDetailID = entity.BillDetailID,
                        IsPreProBind = entity.IsPreProBind,
                        Timeout = 1 // 设置为过期
                    }).ToList();

                    _db.Ask_DataFTOut.AddRange(outEntities);
                    _db.Ask_DataFT.RemoveRange(entities);
                }

                await _db.SaveChangesAsync();
                return ResultModel<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"迁移数据失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 迁移附件数据
        /// </summary>
        private async Task<ResultModel<bool>> MigrateDataFJAsync(List<int> ids, bool fromExpired, string? currentUser)
        {
            try
            {
                if (fromExpired)
                {
                    // 从已过期表迁移到未过期表
                    var outEntities = await _db.Ask_DataFJOut.Where(x => ids.Contains(x.ID)).ToListAsync();

                    var newEntities = outEntities.Select(outEntity => new Ask_DataFJ
                    {
                        Source = outEntity.Source,
                        AskDate = outEntity.AskDate,
                        AskProjName = outEntity.AskProjName,
                        FJType = outEntity.FJType,
                        FJVersion = outEntity.FJVersion,
                        Unit = outEntity.Unit,
                        Num = outEntity.Num,
                        Price = outEntity.Price,
                        Supplier = outEntity.Supplier,
                        ProjDay = outEntity.ProjDay,
                        Day1 = outEntity.Day1,
                        Day2 = outEntity.Day2,
                        Day3 = outEntity.Day3,
                        Memo1 = outEntity.Memo1,
                        DoUser = currentUser,
                        DoDate = DateTime.Now,
                        Memo = outEntity.Memo,
                        PriceRatio = outEntity.PriceRatio,
                        BillDetailID = outEntity.BillDetailID,
                        DN = outEntity.DN,
                        PN = outEntity.PN,
                        OrdLJ = outEntity.OrdLJ,
                        IsPreProBind = outEntity.IsPreProBind,
                        Timeout = -1 // 设置为有效
                    }).ToList();

                    _db.Ask_DataFJ.AddRange(newEntities);
                    _db.Ask_DataFJOut.RemoveRange(outEntities);
                }
                else
                {
                    // 从未过期表迁移到已过期表
                    var entities = await _db.Ask_DataFJ.Where(x => ids.Contains(x.ID)).ToListAsync();

                    var outEntities = entities.Select(entity => new Ask_DataFJOut
                    {
                        Source = entity.Source,
                        AskDate = entity.AskDate,
                        AskProjName = entity.AskProjName,
                        FJType = entity.FJType,
                        FJVersion = entity.FJVersion,
                        Unit = entity.Unit,
                        Num = entity.Num,
                        Price = entity.Price,
                        Supplier = entity.Supplier,
                        ProjDay = entity.ProjDay,
                        Day1 = entity.Day1,
                        Day2 = entity.Day2,
                        Day3 = entity.Day3,
                        Memo1 = entity.Memo1,
                        DoUser = currentUser,
                        DoDate = DateTime.Now,
                        Memo = entity.Memo,
                        PriceRatio = entity.PriceRatio,
                        BillDetailID = entity.BillDetailID,
                        DN = entity.DN,
                        PN = entity.PN,
                        OrdLJ = entity.OrdLJ,
                        IsPreProBind = entity.IsPreProBind,
                        Timeout = 1 // 设置为过期
                    }).ToList();

                    _db.Ask_DataFJOut.AddRange(outEntities);
                    _db.Ask_DataFJ.RemoveRange(entities);
                }

                await _db.SaveChangesAsync();
                return ResultModel<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"迁移数据失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 延长数据有效期
        /// </summary>
        private async Task<ResultModel<bool>> ExtendDataTimeoutAsync(List<int> ids, int extendDays, string? currentUser, string entityType)
        {
            try
            {
                switch (entityType.ToUpper())
                {
                    case "DATAFT":
                        var ftEntities = await _db.Ask_DataFT.Where(x => ids.Contains(x.ID)).ToListAsync();
                        SetTimeoutAndBind(ftEntities, -extendDays, ftEntities.First().IsPreProBind);
                        foreach (var entity in ftEntities)
                        {
                            entity.DoUser = currentUser;
                            entity.DoDate = DateTime.Now;
                        }
                        break;

                    case "DATAFJ":
                        var fjEntities = await _db.Ask_DataFJ.Where(x => ids.Contains(x.ID)).ToListAsync();
                        SetTimeoutAndBind(fjEntities, -extendDays, fjEntities.First().IsPreProBind);
                        foreach (var entity in fjEntities)
                        {
                            entity.DoUser = currentUser;
                            entity.DoDate = DateTime.Now;
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
                return ResultModel<bool>.Error($"延长有效期失败：{ex.Message}");
            }
        }
        /// <summary>
        /// 装饰价格查询结果的派生显示字段
        /// </summary>
        /// <typeparam name="T">实现了IDataItemDto接口的数据传输对象类型</typeparam>
        /// <param name="items">结果集</param>
        private void DecorateDataList<T>(IEnumerable<T> items) where T : IDataItemDto
        {
            foreach (var item in items)
            {
                item.IsPreProBindText = ZKLT25Profile.GetPreProBindText(item.IsPreProBind);
                item.PriceStatusText = ZKLT25Profile.GetPriceStatusText(item.Timeout);
                item.AvailableActions = ZKLT25Profile.GetAvailableActions(item.Timeout);
                item.DocNameStatus = GetUploadStatusText(item.DocName);
                item.FileNameStatus = GetUploadStatusText(item.FileName);
            }
        }

        /// <summary>
        /// 阀体价格查询：应用筛选条件
        /// </summary>
        /// <param name="query">查询对象</param>
        /// <param name="qto">查询参数</param>
        /// <param name="includeExpiredFilter">是否包含过期状态筛选</param>
        private static IQueryable<Ask_DataFTDto> ApplyFTFilters(IQueryable<Ask_DataFTDto> query, Ask_DataFTQto qto, bool includeExpiredFilter = true)
        {
            if (qto.BillID.HasValue)
            {
                query = query.Where(x => x.BillID == qto.BillID.Value);
            }

            if (!string.IsNullOrWhiteSpace(qto.AskProjName))
            {
                query = query.Where(x => x.AskProjName!.Contains(qto.AskProjName));
            }

            if (!string.IsNullOrWhiteSpace(qto.SuppName))
            {
                query = query.Where(x => x.SuppName.Contains(qto.SuppName));
            }
            if (qto.SuppID.HasValue)
            {
                query = query.Where(x => x.SuppID == qto.SuppID.Value);
            }

            if (qto.StartDate.HasValue)
            {
                query = query.Where(x => x.AskDate >= qto.StartDate.Value);
            }

            if (qto.EndDate.HasValue)
            {
                query = query.Where(x => x.AskDate <= qto.EndDate.Value);
            }

            if (includeExpiredFilter && qto.IsExpired.HasValue)
            {
                if (qto.IsExpired.Value)
                {
                    query = query.Where(x => x.Timeout > 0);
                }
                else
                {
                    query = query.Where(x => x.Timeout <= 0);
                }
            }

            // 阀体特有筛选条件
            if (!string.IsNullOrWhiteSpace(qto.OrdVersion))
            {
                query = query.Where(x => x.OrdVersion!.Contains(qto.OrdVersion));
            }

            if (!string.IsNullOrWhiteSpace(qto.OrdDN))
            {
                query = query.Where(x => x.OrdDN!.Contains(qto.OrdDN));
            }

            if (!string.IsNullOrWhiteSpace(qto.OrdPN))
            {
                query = query.Where(x => x.OrdPN!.Contains(qto.OrdPN));
            }

            if (!string.IsNullOrWhiteSpace(qto.OrdFT))
            {
                query = query.Where(x => x.OrdFT!.Contains(qto.OrdFT));
            }

            return query;
        }

        /// <summary>
        /// 附件价格查询：应用筛选条件
        /// </summary>
        /// <param name="query">查询对象</param>
        /// <param name="qto">查询参数</param>
        /// <param name="includeExpiredFilter">是否包含过期状态筛选</param>
        private static IQueryable<Ask_DataFJDto> ApplyFJFilters(IQueryable<Ask_DataFJDto> query, Ask_DataFJQto qto, bool includeExpiredFilter = true)
        {
            if (qto.BillID.HasValue)
            {
                query = query.Where(x => x.BillID == qto.BillID.Value);
            }

            if (!string.IsNullOrWhiteSpace(qto.AskProjName))
            {
                query = query.Where(x => x.AskProjName!.Contains(qto.AskProjName));
            }

            if (!string.IsNullOrWhiteSpace(qto.SuppName))
            {
                query = query.Where(x => x.SuppName.Contains(qto.SuppName));
            }
            if (qto.SuppID.HasValue)
            {
                query = query.Where(x => x.SuppID == qto.SuppID.Value);
            }

            if (qto.StartDate.HasValue)
            {
                query = query.Where(x => x.AskDate >= qto.StartDate.Value);
            }

            if (qto.EndDate.HasValue)
            {
                query = query.Where(x => x.AskDate <= qto.EndDate.Value);
            }

            if (includeExpiredFilter && qto.IsExpired.HasValue)
            {
                if (qto.IsExpired.Value)
                {
                    query = query.Where(x => x.Timeout > 0);
                }
                else
                {
                    query = query.Where(x => x.Timeout <= 0);
                }
            }

            // 附件特有筛选条件
            if (!string.IsNullOrWhiteSpace(qto.FJType))
            {
                query = query.Where(x => x.FJType!.Contains(qto.FJType));
            }
            if (qto.FJTypeId.HasValue)
            {
                query = query.Where(x => x.FJTypeId == qto.FJTypeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(qto.FJVersion))
            {
                query = query.Where(x => x.FJVersion!.Contains(qto.FJVersion));
            }

            return query;
        }


        /// <summary>
        /// 统一排序（默认询价日期倒序）
        /// </summary>
        private static IQueryable<T> ApplySorting<T>(IQueryable<T> query) where T : class
        {
            return query.OrderByDescending(x => EF.Property<DateTime?>(x, "AskDate"));
        }

        /// <summary>
        /// 统一分页
        /// </summary>
        private static Task<PaginationList<T>> PaginateAsync<T>(int pageNumber, int pageSize, IQueryable<T> query) where T : class
        {
            return PaginationList<T>.CreateAsync(pageNumber, pageSize, query);
        }

        /// <summary>
        /// 构建阀体价格-有效数据查询（联表并投影为 Ask_DataFTDto）
        /// </summary>
        private IQueryable<Ask_DataFTDto> BuildDataFTActiveQuery()
        {
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
                                      SuppID = supplier != null ? (int?)supplier.ID : null,
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
                                      FileName = billFile != null ? billFile.FileName : null,
                                      IsInvalid = 1,
                                      Timeout = d.Timeout,
                                      ProjDay = d.ProjDay,
                                      Day1 = d.Day1,
                                      Day2 = d.Day2,
                                      Memo1 = d.Memo1,
                                      BillIDText = billDetail != null ? billDetail.BillID.ToString() : null
                                  };

            return internalQueryFT;
        }

        /// <summary>
        /// 构建阀体价格-过期数据查询（联表并投影为 Ask_DataFTDto）
        /// </summary>
        private IQueryable<Ask_DataFTDto> BuildDataFTExpiredQuery()
        {
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
                                      SuppID = supplier != null ? (int?)supplier.ID : null,
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
                                      FileName = billFile != null ? billFile.FileName : null,
                                      IsInvalid = 0,
                                      Timeout = d.Timeout,
                                      ProjDay = d.ProjDay,
                                      Day1 = d.Day1,
                                      Day2 = d.Day2,
                                      Memo1 = d.Memo1,
                                      BillIDText = billDetail != null ? billDetail.BillID.ToString() : null
                                  };

            return externalQueryFT;
        }
        /// <summary>
        /// 构建附件价格-有效数据查询（联表并投影为 Ask_DataFJDto）
        /// </summary>
        private IQueryable<Ask_DataFJDto> BuildDataFJActiveQuery()
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
                                      SuppID = supplier != null ? (int?)supplier.ID : null,
                                      Price = (double?)d.Price,
                                      BasicsPrice = billPrice != null ? billPrice.BasicsPrice : null,
                                      AddPrice = billPrice != null ? billPrice.AddPrice : null,
                                      AskDate = d.AskDate,
                                      FJType = d.FJType,
                                      FJTypeId = _db.Ask_FJList.Where(t => t.FJType == d.FJType).Select(t => (int?)t.ID).FirstOrDefault(),
                                      FJVersion = d.FJVersion,
                                      Num = d.Num,
                                      Unit = d.Unit,
                                      IsPreProBind = d.IsPreProBind,
                                      DocName = bill.DocName,
                                      FileName = billFile != null ? billFile.FileName : null,
                                      Timeout = d.Timeout,
                                      IsInvalid = 1,
                                      ProjDay = d.ProjDay,
                                      Day1 = d.Day1,
                                      Day2 = d.Day2,
                                      Memo1 = d.Memo1,
                                      BillIDText = billDetail.BillID.ToString()
                                  };

            return internalQueryFJ;
        }

        /// <summary>
        /// 构建附件价格-过期数据查询（联表并投影为 Ask_DataFJDto）
        /// </summary>
        private IQueryable<Ask_DataFJDto> BuildDataFJExpiredQuery()
        {
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
                                      SuppID = supplier != null ? (int?)supplier.ID : null,
                                      Price = (double?)d.Price,
                                      BasicsPrice = billPrice != null ? billPrice.BasicsPrice : null,
                                      AddPrice = billPrice != null ? billPrice.AddPrice : null,
                                      AskDate = d.AskDate,
                                      FJType = d.FJType,
                                      FJTypeId = _db.Ask_FJList.Where(t => t.FJType == d.FJType).Select(t => (int?)t.ID).FirstOrDefault(),
                                      FJVersion = d.FJVersion,
                                      Num = d.Num,
                                      Unit = d.Unit,
                                      IsPreProBind = d.IsPreProBind,
                                      DocName = bill.DocName,
                                      FileName = billFile != null ? billFile.FileName : null,
                                      Timeout = d.Timeout,
                                      IsInvalid = 0,
                                      ProjDay = d.ProjDay,
                                      Day1 = d.Day1,
                                      Day2 = d.Day2,
                                      Memo1 = d.Memo1,
                                      BillIDText = billDetail.BillID.ToString()
                                  };

            return externalQueryFJ;
        }

        /// <summary>
        /// 按数据状态选择（true=未过期，false=已过期，null=合并显示）
        /// </summary>
        private static IQueryable<T> ComposeView<T>(IQueryable<T> internalQuery, IQueryable<T> externalQuery, int? isOutView)
        {
            if (isOutView == 1) return internalQuery;
            if (isOutView == 0) return externalQuery;
            return internalQuery.Union(externalQuery);
        }

        /// <summary>
        /// 批量设置 Timeout 与 IsPreProBind
        /// </summary>
        private static void SetTimeoutAndBind<T>(IEnumerable<T> items, int timeout, int isPreProBind) where T : class
        {
            foreach (var item in items)
            {
                // 使用反射设置属性
                var timeoutProp = typeof(T).GetProperty("Timeout");
                var bindProp = typeof(T).GetProperty("IsPreProBind");

                if (timeoutProp != null && timeoutProp.CanWrite)
                    timeoutProp.SetValue(item, timeout);

                if (bindProp != null && bindProp.CanWrite)
                    bindProp.SetValue(item, isPreProBind);
            }
        }
        #endregion

        #region 导出为excel表
        /// <summary>
        /// 导出附件询价数据为 Excel 文件
        /// </summary>
        public async Task<byte[]> DataFJExcelAsync(Ask_DataFJQto qto)
        {
            return await ExportDataToExcelAsync(
                async () => await GetDataFJPagedListAsync(new Ask_DataFJQto
                {
                    BillID = qto.BillID,
                    AskProjName = qto.AskProjName,
                    SuppName = qto.SuppName,
                    FJType = qto.FJType,
                    FJTypeId = qto.FJTypeId,
                    SuppID = qto.SuppID,
                    FJVersion = qto.FJVersion,
                    StartDate = qto.StartDate,
                    EndDate = qto.EndDate,
                    IsExpired = qto.IsExpired,
                    PageSize = 999999, // 取全量数据
                    PageNumber = 1
                }),
                "附件询价数据",
                new[] {
                    "询价日期", "单据编号", "项目名称", "物品名称", "物品型号", "单位", "数量",
                    "成本价", "供应商名称", "项目交期(天)", "小于10台标准交期(天)", "10台至20台标准交期(天)",
                    "备注1", "报价文件", "明细表"
                },
                (data) => new object[] {
                    data.AskDate?.ToString("yyyy-MM-dd") ?? "",
                    data.BillIDText ?? "",
                    data.AskProjName ?? "",
                    data.FJType ?? "",
                    data.FJVersion ?? "",
                    data.Unit ?? "",
                    data.Num?.ToString() ?? "",
                    data.Price?.ToString("F2") ?? "",
                    data.SuppName ?? "",
                    data.ProjDay ?? "",
                    data.Day1 ?? "",
                    data.Day2 ?? "",
                    data.Memo1 ?? "",
                    data.FileNameStatus,
                    data.DocNameStatus
                }
            );
        }

        /// <summary>
        /// 导出阀体询价数据为 Excel 文件
        /// </summary>
        public async Task<byte[]> DataFTExcelAsync(Ask_DataFTQto qto)
        {
            var headers = new[]
            {
                "询价日期", "单据编号", "项目名称", "阀体名称/名称", "型号", "公称通径DN", "公称压力PN",
                "法兰标准", "上阀盖形式", "阀体材质", "阀内件材质", "填料材质", "额定KV值", "流量特性",
                "泄漏等级", "气源压力/驱动器", "数量", "单位", "供应商名称", "成本价(元)", "基础价(核价)",
                "加价", "项目交期(天)", "小于10台标准交期(天)", "10台至20台标准交期(天)", "备注1",
                "报价文件", "明细表"
            };

            return await ExportDataToExcelAsync(
                async () => await GetDataFTPagedListAsync(new Ask_DataFTQto
                {
                    BillID = qto.BillID,
                    AskProjName = qto.AskProjName,
                    SuppName = qto.SuppName,
                    SuppID = qto.SuppID,
                    OrdVersion = qto.OrdVersion,
                    OrdDN = qto.OrdDN,
                    OrdPN = qto.OrdPN,
                    OrdFT = qto.OrdFT,
                    StartDate = qto.StartDate,
                    EndDate = qto.EndDate,
                    IsExpired = qto.IsExpired,
                    PageSize = 999999,
                    PageNumber = 1
                }),
                "阀体询价数据",
                headers,
                (data) => new object[]
                {
                    data.AskDate?.ToString("yyyy-MM-dd") ?? "",
                    data.BillIDText ?? data.BillID?.ToString() ?? "",
                    data.AskProjName ?? "",
                    data.OrdName ?? "",
                    data.OrdVersion ?? "",
                    data.OrdDN ?? "",
                    data.OrdPN ?? "",
                    data.OrdLJ ?? "",
                    data.OrdFG ?? "",
                    data.OrdFT ?? "",
                    data.OrdFNJ ?? "",
                    data.OrdTL ?? "",
                    data.OrdKV ?? "",
                    data.OrdFlow ?? "",
                    data.OrdLeak ?? "",
                    data.OrdQYDY ?? "",
                    data.Num?.ToString() ?? "",
                    data.OrdUnit ?? "",
                    data.SuppName ?? "",
                    data.Price?.ToString("F2") ?? "",
                    data.BasicsPrice?.ToString("F2") ?? "",
                    data.AddPrice?.ToString("F2") ?? "",
                    data.ProjDay ?? "",
                    data.Day1 ?? "",
                    data.Day2 ?? "",
                    data.Memo1 ?? "",
                    data.FileNameStatus,
                    data.DocNameStatus
                }
            );
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
                SetTimeoutAndBind(dataFTList, timeout, isPreProBind);
                SetTimeoutAndBind(dataFTOutList, timeout, isPreProBind);
                SetTimeoutAndBind(dataFJList, timeout, isPreProBind);
                SetTimeoutAndBind(dataFJOutList, timeout, isPreProBind);

                // 处理报价文件上传
                if (cto.QuoteFile != null && cto.BillDetailIDs.Any())
                {
                    await UploadQuoteFileAsync(cto.QuoteFile, cto.BillDetailIDs.First(), currentUser);
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
        /// 关闭项目（将项目状态从发起0改为已关闭-1）
        /// </summary>
        /// <param name="billId">项目ID</param>
        /// <param name="currentUser">当前用户</param>
        /// <returns>操作结果</returns>
        public async Task<ResultModel<int>> CloseProjectAsync(int billId, string? currentUser)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // 更新Ask_Bill状态
                var bill = await _db.Ask_Bill
                    .FirstOrDefaultAsync(x => x.BillID == billId && x.BillState == 0);

                bill.BillState = -1;

                // 更新关联的Ask_BillDetail状态
                var billDetails = await _db.Ask_BillDetail
                    .Where(x => x.BillID == billId && x.State == 0)
                    .ToListAsync();

                foreach (var detail in billDetails)
                {
                    detail.State = -1; 
                    
                    // 记录明细状态变更日志
                    var billLog = new Ask_BillLog
                    {
                        BillDetailID = detail.ID,
                        State = -1,
                        KDate = DateTime.Now,
                        KUser = currentUser
                    };
                    _db.Ask_BillLog.Add(billLog);
                }

                // 更新关联的AskDay_Bill状态
                var dayBills = await _db.AskDay_Bill
                    .Where(x => x.BillID == billId && x.BillState == 0)
                    .ToListAsync();

                foreach (var dayBill in dayBills)
                {
                    dayBill.BillState = -1;
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return ResultModel<int>.Ok(billDetails.Count + dayBills.Count);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ResultModel<int>.Error($"关闭项目失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 上传报价文件
        /// </summary>
        /// <param name="file">上传的文件</param>
        /// <param name="billDetailId">明细ID</param>
        /// <param name="currentUser">当前用户</param>
        private async Task UploadQuoteFileAsync(IFormFile file, int billDetailId, string? currentUser)
        {
            var fileName = file.FileName;
            var existingFile = await _db.Ask_BillFile
                .FirstOrDefaultAsync(x => x.BillDetailID == billDetailId);

            if (existingFile != null)
            {
                // 更新现有记录
                existingFile.FileName = fileName;
            }
            else
            {
                // 创建新记录
                var billFile = new Ask_BillFile
                {
                    BillDetailID = billDetailId,
                    FileName = fileName,
                    State = 1
                };
                _db.Ask_BillFile.Add(billFile);
            }

            // 写入文件上传日志
            var fileLog = new Ask_BillFileLog
            {
                BillDetailID = billDetailId,
                FileName = fileName,
                CreationDate = DateTime.Now,
                CreationPreson = currentUser
            };
            _db.Ask_BillFileLog.Add(fileLog);
        }


        #endregion

        #region 采购成本维护
        /// <summary>
        /// 分页查询采购成本列表
        /// </summary>
        public async Task<ResultModel<PaginationList<Ask_CGPriceValueDto>>> GetCGPagedListAsync(Ask_CGPriceValueQto qto)
        {
            try
            {
                var query = _db.Ask_CGPriceValue.AsQueryable();

                // 搜索关键字过滤
                if (!string.IsNullOrWhiteSpace(qto.Version))
                {
                    query = query.Where(x => x.Version != null && x.Version.Contains(qto.Version));
                }

                if (!string.IsNullOrWhiteSpace(qto.Name))
                {
                    query = query.Where(x => x.Name != null && x.Name.Contains(qto.Name));
                }

                if (!string.IsNullOrWhiteSpace(qto.Type))
                {
                    query = query.Where(x => x.Type != null && x.Type.Contains(qto.Type));
                }

                if (!string.IsNullOrWhiteSpace(qto.Customer))
                {
                    query = query.Where(x => x.Customer != null && x.Customer.Contains(qto.Customer));
                }

                // 有效性筛选
                if (qto.IsValid.HasValue && qto.IsValid.Value)
                {
                    var currentDate = DateTime.Now;
                    query = query.Where(x => x.ExpireTime == null || x.ExpireTime > currentDate);
                }
                // 当 IsValid = null 时显示全部数据

                // 排序
                query = query.OrderBy(x => x.Id);

                // 分页
                var result = await PaginationList<Ask_CGPriceValueDto>.CreateAsync(qto.PageNumber, qto.PageSize, query.ProjectTo<Ask_CGPriceValueDto>(_mapper.ConfigurationProvider));
                return ResultModel<PaginationList<Ask_CGPriceValueDto>>.Ok(result);
            }
            catch (Exception ex)
            {
                return ResultModel<PaginationList<Ask_CGPriceValueDto>>.Error($"查询失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 创建采购成本记录
        /// </summary>
        public async Task<ResultModel<bool>> CreateCGAsync(Ask_CGPriceValueCto cto)
        {
            try
            {
                var entity = _mapper.Map<Ask_CGPriceValue>(cto);
                
                // 如果没有设置有效期，默认3650天
                if (!entity.ExpireTime.HasValue)
                {
                    entity.ExpireTime = DateTime.Now.AddDays(3650);
                }

                // 验证字段填写规则
                var validationResult = ValidateCGFields(entity.Type, entity.DN, entity.PN, entity.ordQY);
                if (!validationResult.Success)
                {
                    return validationResult;
                }

                _db.Ask_CGPriceValue.Add(entity);
                await _db.SaveChangesAsync();

                return ResultModel<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"创建失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 删除采购成本记录
        /// </summary>
        public async Task<ResultModel<bool>> DeleteCGAsync(int id)
        {
            try
            {
                var entity = await _db.Ask_CGPriceValue.FindAsync(id);
                _db.Ask_CGPriceValue.Remove(entity);
                await _db.SaveChangesAsync();

                return ResultModel<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"删除失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 更新采购成本记录
        /// </summary>
        public async Task<ResultModel<bool>> UpdateCGAsync(int id, Ask_CGPriceValueUto uto)
        {
            try
            {
                var entity = await _db.Ask_CGPriceValue.FindAsync(id);
                if (entity == null)
                {
                    return ResultModel<bool>.Error("记录不存在");
                }

                // 验证字段填写规则
                var validationResult = ValidateCGFields(entity.Type, uto.DN, uto.PN, uto.ordQY);
                if (!validationResult.Success)
                {
                    return validationResult;
                }

                // 更新字段
                entity.Price = uto.Price;
                entity.AddPrice = uto.AddPrice ?? 0;
                entity.DN = uto.DN;
                entity.PN = uto.PN;
                entity.ordQY = uto.ordQY;
                entity.ExpireTime = uto.ExpireTime;
                entity.PriceMemo = uto.PriceMemo;
                entity.Customer = uto.Customer;
                entity.SuppId = uto.SuppId;

                await _db.SaveChangesAsync();

                return ResultModel<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"更新失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 验证采购成本字段填写规则
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="dn">口径</param>
        /// <param name="pn">压力</param>
        /// <param name="ordQY">气源压力</param>
        /// <returns>验证结果</returns>
        private ResultModel<bool> ValidateCGFields(string? type, string? dn, string? pn, string? ordQY)
        {
            if (type == "执行机构")
            {
                // 执行机构类型：气源压力可以填写
                if (!string.IsNullOrWhiteSpace(dn) || !string.IsNullOrWhiteSpace(pn))
                {
                    return ResultModel<bool>.Error("字段验证失败");
                }
            }
            else if (type == "配对法兰及螺栓螺母")
            {
                // 配对法兰及螺栓螺母类型：DN和PN可以填写
                if (!string.IsNullOrWhiteSpace(ordQY))
                {
                    return ResultModel<bool>.Error("字段验证失败");
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(dn) || !string.IsNullOrWhiteSpace(pn) || !string.IsNullOrWhiteSpace(ordQY))
                {
                    return ResultModel<bool>.Error("字段验证失败");
                }
            }

            return ResultModel<bool>.Ok(true);
        }

        /// <summary>
        /// 导出采购成本数据为Excel文件
        /// </summary>
        public async Task<byte[]> ExportCGExcelAsync(Ask_CGPriceValueQto qto)
        {
            return await ExportDataToExcelAsync(
                async () => await GetCGPagedListAsync(new Ask_CGPriceValueQto
                {
                    Version = qto.Version,
                    Name = qto.Name,
                    Type = qto.Type,
                    Customer = qto.Customer,
                    IsValid = qto.IsValid,
                    PageSize = 999999, // 取全量数据
                    PageNumber = 1
                }),
                "采购成本库数据",
                new[] {
                    "型号", "类型", "口径", "压力", "气源压力", "基础价格", "加价", "截止日期", "备注", "客户"
                },
                (data) => new object[] {
                    data.Version ?? "",
                    data.Type ?? "",
                    data.DN ?? "",
                    data.PN ?? "",
                    data.ordQY ?? "",
                    data.Price?.ToString("F2") ?? "",
                    data.AddPrice?.ToString("F2") ?? "",
                    data.ExpireTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                    data.PriceMemo ?? "",
                    data.Customer ?? ""
                }
            );
        }


        /// <summary>
        /// 导入采购成本数据Excel文件
        /// </summary>
        public async Task<ResultModel<ImportResult>> ImportCGExcelAsync(IFormFile file, bool isReplace = false)
        {
            return await ImportExcelDataAsync(
                file,
                isReplace,
                "采购成本库数据",
                (worksheet, row, errors) =>
                {
                    // 读取Excel行数据
                    var version = worksheet.Cells[row, 0].Value?.ToString()?.Trim();
                    var type = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                    var dn = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                    var pn = worksheet.Cells[row, 3].Value?.ToString()?.Trim();
                    var ordQY = worksheet.Cells[row, 4].Value?.ToString()?.Trim();
                    var priceStr = worksheet.Cells[row, 5].Value?.ToString()?.Trim();
                    var addPriceStr = worksheet.Cells[row, 6].Value?.ToString()?.Trim();
                    var expireTimeStr = worksheet.Cells[row, 7].Value?.ToString()?.Trim();
                    var priceMemo = worksheet.Cells[row, 8].Value?.ToString()?.Trim();
                    var customer = worksheet.Cells[row, 9].Value?.ToString()?.Trim();

                    // 验证必填字段
                    if (string.IsNullOrWhiteSpace(version) || string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(priceStr))
                    {
                        errors.Add(new ImportError { RowNumber = row + 1, ErrorMessage = "必填字段缺失" });
                        return null;
                    }

                    // 验证价格格式
                    if (!double.TryParse(priceStr, out var price) || price <= 0)
                    {
                        errors.Add(new ImportError { RowNumber = row + 1, ErrorMessage = "价格格式错误" });
                        return null;
                    }

                    // 验证业务规则
                    var validationResult = ValidateCGFields(type, dn, pn, ordQY);
                    if (!validationResult.Success)
                    {
                        errors.Add(new ImportError { RowNumber = row + 1, ErrorMessage = "字段验证失败" });
                        return null;
                    }

                    // 解析其他字段
                    double.TryParse(addPriceStr, out var addPrice);
                    DateTime? expireTime = null;
                    if (!string.IsNullOrWhiteSpace(expireTimeStr))
                    {
                        if (DateTime.TryParse(expireTimeStr, out var parsedTime))
                        {
                            expireTime = parsedTime;
                        }
                        else
                        {
                            errors.Add(new ImportError { RowNumber = row + 1, ErrorMessage = "日期格式错误" });
                            return null;
                        }
                    }

                    // 如果没有设置有效期，默认3650天
                    if (!expireTime.HasValue)
                    {
                        expireTime = DateTime.Now.AddDays(3650);
                    }

                    // 创建实体
                    return new Ask_CGPriceValue
                    {
                        Version = version,
                        Type = type,
                        DN = dn,
                        PN = pn,
                        ordQY = ordQY,
                        Price = price,
                        AddPrice = addPrice,
                        ExpireTime = expireTime,
                        PriceMemo = priceMemo,
                        Customer = customer
                    };
                },
                async (isReplace, validData) =>
                {
                    if (isReplace)
                    {
                        // 全量替换模式
                        _db.Ask_CGPriceValue.RemoveRange(_db.Ask_CGPriceValue);
                        await _db.SaveChangesAsync();
                    }

                    // 批量插入有效数据
                    if (validData.Any())
                    {
                        _db.Ask_CGPriceValue.AddRange(validData);
                        await _db.SaveChangesAsync();
                    }
                }
            );
        }

        #endregion

        #region 通用导入导出方法

        /// <summary>
        /// 验证上传的Excel文件
        /// </summary>
        /// <param name="file">上传的文件</param>
        /// <returns>验证结果</returns>
        private ResultModel<bool> ValidateExcelFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return ResultModel<bool>.Error("请选择要导入的Excel文件");
            }

            if (!file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xls"))
            {
                return ResultModel<bool>.Error("请选择Excel文件格式");
            }

            return ResultModel<bool>.Ok(true);
        }

        /// <summary>
        /// 读取Excel文件内容
        /// </summary>
        /// <param name="file">上传的文件</param>
        /// <returns>工作表</returns>
        private Worksheet ReadExcelFile(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var workbook = new Workbook(stream);
            return workbook.Worksheets[0];
        }

        /// <summary>
        /// 通用Excel数据导入方法
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="file">上传的Excel文件</param>
        /// <param name="isReplace">是否全量替换</param>
        /// <param name="sheetName">工作表名称（用于错误提示）</param>
        /// <param name="rowParser">行数据解析函数</param>
        /// <param name="dataSaver">数据保存函数</param>
        /// <returns>导入结果</returns>
        private async Task<ResultModel<ImportResult>> ImportExcelDataAsync<T>(
            IFormFile file,
            bool isReplace,
            string sheetName,
            Func<Worksheet, int, List<ImportError>, T?> rowParser,
            Func<bool, List<T>, Task> dataSaver) where T : class
        {
            var result = new ImportResult();
            
            try
            {
                // 验证文件
                var fileValidation = ValidateExcelFile(file);
                if (!fileValidation.Success)
                {
                    return ResultModel<ImportResult>.Error(fileValidation.Message);
                }

                // 读取Excel文件
                var worksheet = ReadExcelFile(file);
                var validData = new List<T>();
                var errors = new List<ImportError>();

                for (int row = 1; row <= worksheet.Cells.MaxDataRow; row++)
                {
                    try
                    {
                        var entity = rowParser(worksheet, row, errors);
                        if (entity != null)
                        {
                            validData.Add(entity);
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add(new ImportError { RowNumber = row + 1, ErrorMessage = $"数据解析失败：{ex.Message}" });
                    }
                }

                // 使用事务保存数据
                using var transaction = await _db.Database.BeginTransactionAsync();
                try
                {
                    await dataSaver(isReplace, validData);
                    await transaction.CommitAsync();

                    // 设置返回结果
                    result.SuccessCount = validData.Count;
                    result.FailCount = errors.Count;
                    result.Errors = errors;

                    return ResultModel<ImportResult>.Ok(result);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return ResultModel<ImportResult>.Error($"保存数据失败：{ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return ResultModel<ImportResult>.Error($"导入失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 通用数据导出到Excel方法
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="dataProvider">数据提供函数</param>
        /// <param name="sheetName">工作表名称</param>
        /// <param name="headers">表头数组</param>
        /// <param name="dataMapping">数据映射函数</param>
        /// <returns>Excel文件字节数组</returns>
        private async Task<byte[]> ExportDataToExcelAsync<T>(
            Func<Task<ResultModel<PaginationList<T>>>> dataProvider,
            string sheetName,
            string[] headers,
            Func<T, object[]> dataMapping) where T : class
        {
            try
            {
                var result = await dataProvider();

                if (!result.Success || result.Data == null)
                {
                    throw new Exception(result.Message ?? "获取数据失败");
                }

                return CreateExcelFile(sheetName, headers, result.Data, dataMapping);
            }
            catch (Exception ex)
            {
                throw new Exception($"导出失败：{ex.Message}", ex);
            }
        }


        /// <summary>
        /// 通用Excel文件创建方法
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="sheetName">工作表名称</param>
        /// <param name="headers">表头数组</param>
        /// <param name="dataList">数据列表</param>
        /// <param name="dataMapping">数据映射函数</param>
        /// <returns>Excel文件字节数组</returns>
        private byte[] CreateExcelFile<T>(string sheetName, string[] headers, PaginationList<T> dataList, Func<T, object[]> dataMapping)
        {
            var workbook = new Workbook();
            var worksheet = workbook.Worksheets[0];
            worksheet.Name = sheetName;

            // 设置表头
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cells[0, i];
                cell.Value = headers[i];
                var style = cell.GetStyle();
                style.Font.IsBold = true;
                style.HorizontalAlignment = TextAlignmentType.Center;
                cell.SetStyle(style);
            }

            // 填充数据
            for (int row = 0; row < dataList.Count; row++)
            {
                var data = dataList[row];
                var values = dataMapping(data);
                for (int col = 0; col < values.Length; col++)
                {
                    worksheet.Cells[row + 1, col].Value = values[col];
                }
            }

            // 自动列宽
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.AutoFitColumn(i);
            }

            using var stream = new MemoryStream();
            workbook.Save(stream, SaveFormat.Xlsx);
            return stream.ToArray();
        }
        #endregion
    }
}