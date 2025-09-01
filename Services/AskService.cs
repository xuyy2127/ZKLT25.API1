using Aspose.Cells;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ZKLT25.API.EntityFrameworkCore;
using ZKLT25.API.Helper;
using ZKLT25.API.IServices;
using ZKLT25.API.IServices.Dtos;
using ZKLT25.API.Models;

namespace ZKLT25.API.Services
{
    public class AskService : IAskService
    {
        #region 构造函数与常量 (Constructor & Constants)

        // 常量定义，避免魔术字符串
        private const string DataTypeValve = "阀体";
        private const string DataTypeAccessory = "附件";
        private const string EntityTypeDataFT = "DATAFT";
        private const string EntityTypeDataFJ = "DATAFJ";

        // 业务常量定义
        private const int DefaultNewPriceValidityDays = 365; // 新报价默认有效期：365天
        private const int DefaultReactivatePriceValidityDays = 30; // 重新激活的报价默认有效期：30天

        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AskService> _logger;
        private readonly IDateTimeProvider _dateTimeProvider;

        public AskService(AppDbContext db, IMapper mapper, IMemoryCache cache, ILogger<AskService> logger, IDateTimeProvider dateTimeProvider)
        {
            _db = db;
            _mapper = mapper;
            _cache = cache;
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
        }

        #endregion

        #region 1. 基础数据与配置 (Master Data & Configuration)

        #region 1.1. 产品与物料维护 (Product & Material Maintenance)

        /// <summary>
        /// 分页查询阀体型号列表
        /// </summary>
        public async Task<ResultModel<PaginationList<Ask_FTListDto>>> GetFTPagedListAsync(Ask_FTListQto qto)
        {
            try
            {
                var query = _db.Ask_FTList.AsNoTracking().AsQueryable();

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
            using var transaction = await _db.Database.BeginTransactionAsync();
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

                // 记录修改日志
                AddLog(
                    mainId: id,
                    dataType: DataTypeValve,
                    partType: DataTypeValve,
                    partVersion: uto.FTVersion,
                    partName: uto.FTName,
                    ratio: uto.ratio ?? 1.0, // 如果为空则使用默认值1.0
                    currentUser: currentUser
                );

                await _db.SaveChangesAsync(); // 关键：保存所有更改
                await transaction.CommitAsync();

                var model = ResultModel<bool>.Ok(true);
                model.Warning = GetRatioWarning(uto.isWG, uto.ratio);
                return model;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
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
        /// 批量更新阀体系数
        /// </summary>
        public async Task<ResultModel<bool>> BatchUpdateFTRatioAsync(List<int> ids, double ratio, string? currentUser)
        {
            try
            {
                // 日志工厂：为阀体创建日志
                Func<Ask_FTList, Ask_FTFJListLog> logFactory = entity => CreateLog(
                        mainId: entity.ID,
                    dataType: DataTypeValve,
                    partType: DataTypeValve,
                        partVersion: entity.FTVersion,
                        partName: entity.FTName,
                        ratio: ratio,
                        currentUser: currentUser
                    );

                // 警告工厂：为阀体生成警告
                Func<Ask_FTList, string?> warningFactory = entity => GetRatioWarning(entity.isWG, entity.ratio);

                return await BatchUpdateRatioAsync<Ask_FTList>(ids, ratio, currentUser, logFactory, warningFactory);
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"批量更新失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 分页查询附件列表
        /// </summary>
        public async Task<ResultModel<PaginationList<Ask_FJListDto>>> GetFJPagedListAsync(Ask_FJListQto qto)
        {
            try
            {
                var query = _db.Ask_FJList.AsNoTracking().AsQueryable();

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
        /// 批量更新附件系数
        /// </summary>
        public async Task<ResultModel<bool>> BatchUpdateFJRatioAsync(List<int> ids, double ratio, string? currentUser)
        {
            try
            {
                // 日志工厂：为附件创建日志
                Func<Ask_FJList, Ask_FTFJListLog> logFactory = entity => CreateLog(
                        mainId: entity.ID,
                    dataType: DataTypeAccessory,
                    partType: DataTypeAccessory,
                        partVersion: entity.FJType,
                        partName: entity.FJType,
                        ratio: ratio,
                        currentUser: currentUser
                    );

                return await BatchUpdateRatioAsync<Ask_FJList>(ids, ratio, currentUser, logFactory);
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"批量更新失败：{ex.Message}");
            }
        }

        #endregion

        #region 1.2. 供应商维护 (Supplier Maintenance)

        /// <summary>
        /// 分页查询供应商信息
        /// </summary>
        public async Task<ResultModel<PaginationList<Ask_SupplierDto>>> GetSPPagedListAsync(Ask_SupplierQto qto)
        {
            try
            {
                var query = _db.Ask_Supplier.AsNoTracking().AsQueryable();

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
                entity.KDate = GetCurrentTime();
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

        #region 1.3. 供应关系配置 (Sourcing Rules Configuration)

        /// <summary>
        /// 获取供应商附件配置页面数据
        /// </summary>
        public async Task<ResultModel<List<SPFJPageDto>>> GetSPFJPageAsync(int supplierId)
        {
            try
            {
                // 使用LEFT JOIN查询：所有附件 LEFT JOIN 该供应商的供货关系
                var result = await (from fj in _db.Ask_FJList.AsNoTracking()
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
            using var transaction = await _db.Database.BeginTransactionAsync();
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
                await transaction.CommitAsync();
                return ResultModel<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ResultModel<bool>.Error($"更新失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取供应商阀体配置页面数据
        /// </summary>
        public async Task<ResultModel<List<SPFTPageDto>>> GetSPFTPageAsync(int supplierId)
        {
            try
            {
                // 使用LEFT JOIN查询：所有阀体 LEFT JOIN 该供应商的供货关系
                var result = await (from ft in _db.Ask_FTList.AsNoTracking()
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
            using var transaction = await _db.Database.BeginTransactionAsync();
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
                await transaction.CommitAsync();
                return ResultModel<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ResultModel<bool>.Error($"更新失败: {ex.Message}");
            }
        }

        #endregion

        #region 1.4. 采购成本库 (Purchase Cost Library)

        /// <summary>
        /// 分页查询采购成本列表
        /// </summary>
        public async Task<ResultModel<PaginationList<Ask_CGPriceValueDto>>> GetCGPagedListAsync(Ask_CGPriceValueQto qto)
        {
            try
            {
                var query = _db.Ask_CGPriceValue.AsNoTracking().AsQueryable();

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
                    var currentDate = GetCurrentTime();
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
                    entity.ExpireTime = GetCurrentTime().AddDays(3650);
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
                if (entity == null)
                {
                    return ResultModel<bool>.Error("记录不存在");
                }

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

        #endregion

        #endregion

        #region 2. 核心询价流程 (Core Inquiry Process)

        #region 2.1. 询价单据管理 (Inquiry Bill Management)

        /// <summary>
        /// 获取询价分页数据
        /// </summary>
        public async Task<ResultModel<PaginationList<Ask_BillDto>>> GetBillPagedListAsync(Ask_BillQto qto)
        {
            try
            {
                var query = from bill in _db.Ask_Bill.AsNoTracking()
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
        /// </summary>
        public async Task<ResultModel<List<Ask_BillDetailDto>>> GetBillDetailsAsync(int billId)
        {
            try
            {
                var query = from detail in _db.Ask_BillDetail.AsNoTracking().Where(d => d.BillID == billId)

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

                if (bill == null)
                {
                    return ResultModel<int>.Error("项目不存在或状态不正确");
                }

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
                        KDate = GetCurrentTime(),
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

        #endregion

        #region 2.2. 价格与备注录入 (Pricing & Remark Entry)

        /// <summary>
        /// 录入价格备注
        /// </summary>
        /// <param name="cto">价格录入请求</param>
        /// <param name="currentUser">当前用户</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileStream">文件流</param>
        /// <returns>返回影响的记录数</returns>
        public async Task<ResultModel<int>> SetPriceRemarkAsync(BillPriceCto cto, string? currentUser, string? fileName = null, Stream? fileStream = null)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // 步骤1: 获取和验证数据
                var targetDetails = await GetAndValidateTargetDetails(cto.BillDetailIDs);
                var supplierIdStr = cto.SuppID?.ToString();
                var timeout = -(cto.ValidityDays ?? DefaultNewPriceValidityDays);
                var isPreProBind = cto.IsPreProBind ?? 0;

                // 步骤2: 获取数据仓库表数据
                var (dataFTList, dataFTOutList, dataFJList, dataFJOutList) = await GetDataWarehouseData(targetDetails, supplierIdStr);

                // 步骤3: 更新价格和明细状态
                await UpdatePricesAndDetails(targetDetails, cto, currentUser);

                // 步骤4: 更新关联的数据仓库表
                UpdateDataWarehouseTables(dataFTList, dataFTOutList, dataFJList, dataFJOutList, timeout, isPreProBind);

                // 步骤5: 处理文件上传
                if (fileStream != null && !string.IsNullOrEmpty(fileName) && cto.BillDetailIDs.Any())
                {
                    await UploadQuoteFileAsync(fileName, fileStream, cto.BillDetailIDs.First(), currentUser);
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

        // SetPriceRemarkAsync 的私有辅助方法
        private async Task<List<Ask_BillDetail>> GetAndValidateTargetDetails(List<int> billDetailIds)
        {
            var targetDetails = await _db.Ask_BillDetail
                .Where(x => billDetailIds.Contains(x.ID) && x.State == 0)
                .ToListAsync();

            if (!targetDetails.Any())
            {
                throw new InvalidOperationException("没有找到有效的待处理明细");
            }

            // 批量操作验证Type和Version是否一致
            if (billDetailIds.Count > 1)
            {
                var firstRecord = targetDetails.First();
                var differentRecords = targetDetails
                    .Where(x => x.Type != firstRecord.Type || x.Version != firstRecord.Version)
                    .ToList();

                if (differentRecords.Any())
                {
                    throw new InvalidOperationException($"类型和型号必须相同。首个记录：{firstRecord.Type}-{firstRecord.Version}");
                }
            }

            return targetDetails;
        }

        private async Task<(List<Ask_DataFT> dataFTList, List<Ask_DataFTOut> dataFTOutList, List<Ask_DataFJ> dataFJList, List<Ask_DataFJOut> dataFJOutList)>
            GetDataWarehouseData(List<Ask_BillDetail> targetDetails, string? supplierIdStr)
        {
            var valveBodyIds = targetDetails.Where(x => x.Type == DataTypeValve).Select(x => x.ID).ToList();
            var attachmentIds = targetDetails.Where(x => x.Type != DataTypeValve).Select(x => x.ID).ToList();

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

            return (dataFTList, dataFTOutList, dataFJList, dataFJOutList);
        }

        private async Task UpdatePricesAndDetails(List<Ask_BillDetail> targetDetails, BillPriceCto cto, string? currentUser)
        {
            var allDetailIDs = targetDetails.Select(d => d.ID).ToList();
            var existingBillPrices = await _db.Ask_BillPrice
                .Where(p => allDetailIDs.Contains(p.BillDetailID.Value) && p.SuppID == cto.SuppID)
                .ToListAsync();
            var billPriceDict = existingBillPrices.ToDictionary(x => x.BillDetailID.Value);

            foreach (var detail in targetDetails)
            {
                // 创建/更新 BillPrice
                if (!billPriceDict.TryGetValue(detail.ID, out var billPrice))
                {
                    billPrice = new Ask_BillPrice
                    {
                        BillDetailID = detail.ID,
                        SuppID = cto.SuppID,
                        KDate = GetCurrentTime(),
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
                billPrice.KDate = GetCurrentTime();
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
                    KDate = GetCurrentTime(),
                    KUser = currentUser
                };
                _db.Ask_BillLog.Add(billLog);
            }
        }

        private void UpdateDataWarehouseTables(
            List<Ask_DataFT> dataFTList,
            List<Ask_DataFTOut> dataFTOutList,
            List<Ask_DataFJ> dataFJList,
            List<Ask_DataFJOut> dataFJOutList,
            int timeout,
            int isPreProBind)
        {
            SetTimeoutAndBindEntities(dataFTList, timeout, isPreProBind);
            SetTimeoutAndBindEntities(dataFTOutList, timeout, isPreProBind);
            SetTimeoutAndBindEntities(dataFJList, timeout, isPreProBind);
            SetTimeoutAndBindEntities(dataFJOutList, timeout, isPreProBind);
        }

        private async Task UploadQuoteFileAsync(string fileName, Stream fileStream, int billDetailId, string? currentUser)
        {
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
                CreationDate = GetCurrentTime(),
                CreationPreson = currentUser
            };
            _db.Ask_BillFileLog.Add(fileLog);
        }


        #endregion

        #endregion

        #region 3. 数据查询与分析 (Data Query & Analysis)

        #region 3.1. 历史价格查询 (Historical Price Query)

        /// <summary>
        /// 分页查阀体价格
        /// </summary>
        public async Task<ResultModel<PaginationList<Ask_DataFTDto>>> GetDataFTPagedListAsync(Ask_DataFTQto qto)
        {
            try
            {
                // 获取 FT 类型映射，避免子查询
                var ftTypeMapping = await GetFTTypeIdMappingAsync();

                IQueryable<Ask_DataFTDto> query;
                
                // 根据过期状态筛选确定查询哪张表
                if (qto.IsExpired.HasValue)
                {
                    if (qto.IsExpired.Value)
                    {
                        query = BuildDataFTExpiredQuery(ftTypeMapping);
                    }
                    else
                    {
                        query = BuildDataFTActiveQuery(ftTypeMapping);
                    }
                }
                else
                {
                    // 显示全部数据：合并两张表
                    var activeQuery = BuildDataFTActiveQuery(ftTypeMapping);
                    var expiredQuery = BuildDataFTExpiredQuery(ftTypeMapping);
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
                // 预加载附件类型ID映射（带缓存）
                var fjTypeMapping = await GetFJTypeIdMappingAsync();

                // 根据过期状态筛选确定查询哪张表
                IQueryable<Ask_DataFJDto> query;
                
                if (qto.IsExpired.HasValue)
                {
                    if (qto.IsExpired.Value)
                    {
                        query = BuildDataFJExpiredQuery(fjTypeMapping);
                    }
                    else
                    {
                        query = BuildDataFJActiveQuery(fjTypeMapping);
                    }
                }
                else
                {
                    // 显示全部数据：合并两张表
                    var activeQuery = BuildDataFJActiveQuery(fjTypeMapping);
                    var expiredQuery = BuildDataFJExpiredQuery(fjTypeMapping);
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

        #endregion

        #region 3.2. 日志查询 (Log Query)

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

        /// <summary>
        /// 获取询价的状态日志
        /// </summary>
        public async Task<ResultModel<List<Ask_BillLogDto>>> GetBillLogsAsync(Ask_BillLogQto qto)
        {
            try
            {
                var logs = await (from log in _db.Ask_BillLog.AsNoTracking().Where(x => x.BillDetailID == qto.BillDetailID)
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

        #endregion

        #region 4. 数据导入导出 (Data Exchange)

        #region 4.1. 导出 (Export)

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

        #endregion

        #region 4.2. 导入 (Import)

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
                        expireTime = GetCurrentTime().AddDays(3650);
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
                        // 全量替换模式：先删除所有数据，但不立即保存
                        _db.Ask_CGPriceValue.RemoveRange(_db.Ask_CGPriceValue);
                    }

                    // 批量插入有效数据
                    if (validData.Any())
                    {
                        _db.Ask_CGPriceValue.AddRange(validData);
                    }
                    
                    // 统一保存，确保事务完整性
                    await _db.SaveChangesAsync();
                }
            );
        }

        #endregion

        #endregion

        #region 5. 内部帮助方法 (Private Helpers)

        #region 5.1. 通用业务逻辑 (Common Business Logic)

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
        /// 迁移阀体数据
        /// </summary>
        private async Task<ResultModel<bool>> MigrateDataFTAsync(List<int> ids, bool fromExpired, string? currentUser)
        {
            string sourceTable = fromExpired ? "Ask_DataFTOut" : "Ask_DataFT";
            string targetTable = fromExpired ? "Ask_DataFT" : "Ask_DataFTOut";

            // ！！！重要！！！
            // 如果修改了 Ask_DataFT 或 Ask_DataFTOut 实体，必须手动同步更新下面的字段列表！
            string columns = "Source, AskDate, AskProjName, OrdNum, OrdMed, OrdName, OrdVersion, OrdDN, OrdPN, OrdLJ, OrdFG, OrdFT, OrdFNJ, OrdTL, OrdKV, OrdFlow, OrdLeak, OrdQYDY, OrdUnit, Num, Memo, AskRequire, Price, Supplier, ProjDay, Day1, Day2, Day3, Memo1, PriceRatio, BillDetailID, IsPreProBind, Timeout";
            
            return await MigrateEntityAsync(ids, fromExpired, currentUser, sourceTable, targetTable, columns, DataTypeValve);
        }

        /// <summary>
        /// 迁移附件数据
        /// </summary>
        private async Task<ResultModel<bool>> MigrateDataFJAsync(List<int> ids, bool fromExpired, string? currentUser)
        {
            string sourceTable = fromExpired ? "Ask_DataFJOut" : "Ask_DataFJ";
            string targetTable = fromExpired ? "Ask_DataFJ" : "Ask_DataFJOut";

            // ！！！重要！！！
            // 如果修改了 Ask_DataFJ 或 Ask_DataFJOut 实体，必须手动同步更新下面的字段列表！
            string columns = "Source, AskDate, AskProjName, FJType, FJVersion, Unit, Num, Price, Supplier, ProjDay, Day1, Day2, Day3, Memo1, Memo, PriceRatio, BillDetailID, DN, PN, OrdLJ, IsPreProBind, Timeout";
            
            return await MigrateEntityAsync(ids, fromExpired, currentUser, sourceTable, targetTable, columns, DataTypeAccessory);
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
        /// 延长数据有效期
        /// </summary>
        private async Task<ResultModel<bool>> ExtendDataTimeoutAsync(List<int> ids, int extendDays, string? currentUser, string entityType)
        {
            try
            {
                switch (entityType.ToUpper())
                {
                    case EntityTypeDataFT:
                        await ExtendEntityTimeoutAsync<Ask_DataFT>(ids, extendDays, currentUser);
                        break;
                    case EntityTypeDataFJ:
                        await ExtendEntityTimeoutAsync<Ask_DataFJ>(ids, extendDays, currentUser);
                        break;
                    default:
                        return ResultModel<bool>.Error("无效的实体类型");
                }

                await _db.SaveChangesAsync(); // 在所有实体更新后，一次性保存
                return ResultModel<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Error($"延长有效期失败：{ex.Message}");
            }
        }
        
        /// <summary>
        /// 通用批量更新系数方法
        /// </summary>
        private async Task<ResultModel<bool>> BatchUpdateRatioAsync<TEntity>(
            List<int> ids,
            double ratio,
            string? currentUser,
            Func<TEntity, Ask_FTFJListLog> logFactory,
            Func<TEntity, string?>? warningFactory = null) where TEntity : class, IEntityWithId, IEntityWithRatio
        {
            if (ids == null || !ids.Any())
            {
                return ResultModel<bool>.Error("请选择要更新的记录");
            }

            var entities = await _db.Set<TEntity>().Where(x => ids.Contains(x.ID)).ToListAsync();
            if (!entities.Any())
            {
                return ResultModel<bool>.Error("未找到要更新的记录");
            }

            var logs = new List<Ask_FTFJListLog>();
            string? warning = null;

            foreach (var entity in entities)
            {
                entity.ratio = ratio;
                logs.Add(logFactory(entity));

                if (warning == null && warningFactory != null)
                {
                    warning = warningFactory(entity);
                }
            }

            _db.Ask_FTFJListLog.AddRange(logs);
            await _db.SaveChangesAsync();

            var model = ResultModel<bool>.Ok(true);
            model.Warning = warning;
            return model;
        }

        /// <summary>
        /// 通用数据迁移方法（使用原生SQL优化性能）
        /// </summary>
        private async Task<ResultModel<bool>> MigrateEntityAsync(
            List<int> ids,
            bool fromExpired,
            string? currentUser,
            string sourceTable,
            string targetTable,
            string columns,
            string entityTypeNameForLog)
        {
            if (ids == null || !ids.Any())
            {
                return ResultModel<bool>.Error("没有要迁移的数据");
            }

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // --- 安全的参数化查询 ---
                var parameters = new List<Microsoft.Data.SqlClient.SqlParameter>();
                var paramNames = new List<string>();
                for (int i = 0; i < ids.Count; i++)
                {
                    var paramName = $"@id{i}";
                    paramNames.Add(paramName);
                    parameters.Add(new Microsoft.Data.SqlClient.SqlParameter(paramName, ids[i]));
                }
                var idListForSql = string.Join(",", paramNames);

                // 设置新的超时值
                int newTimeoutValue;
                if (fromExpired) // 从过期 -> 有效
                {
                    // 使用重新激活的默认有效期
                    newTimeoutValue = -DefaultReactivatePriceValidityDays;
                }
                else // 从有效 -> 过期
                {
                    newTimeoutValue = 0;
                }

                // 1. 插入数据（更新Timeout字段）
                var insertSql = $@"
                    INSERT INTO {targetTable} ({columns}, DoUser, DoDate) 
                    SELECT {columns.Replace("Timeout", $"@newTimeoutValue as Timeout")}, @currentUser, @doDate 
                    FROM {sourceTable} 
                    WHERE ID IN ({idListForSql})";

                parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@currentUser", currentUser ?? "系统用户"));
                parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@doDate", GetCurrentTime()));
                parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@newTimeoutValue", newTimeoutValue));
                await _db.Database.ExecuteSqlRawAsync(insertSql, parameters.ToArray());

                // 2. 删除数据
                var deleteSql = $"DELETE FROM {sourceTable} WHERE ID IN ({idListForSql})";
                var idParameters = parameters.Where(p => p.ParameterName.StartsWith("@id")).ToArray();
                await _db.Database.ExecuteSqlRawAsync(deleteSql, idParameters);

                await transaction.CommitAsync();
                return ResultModel<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "迁移 {EntityType} 数据失败。IDs: {ids}", entityTypeNameForLog, string.Join(",", ids));
                return ResultModel<bool>.Error("迁移数据失败，请查看系统日志。");
            }
        }

        /// <summary>
        /// 通用延长实体有效期方法
        /// </summary>
        private async Task<ResultModel<bool>> ExtendEntityTimeoutAsync<T>(List<int> ids, int extendDays, string? currentUser)
            where T : class, IEntityWithId, ITimeoutBindable
        {
            var entities = await _db.Set<T>().Where(x => ids.Contains(x.ID)).ToListAsync();
            foreach (var entity in entities)
            {
                var currentTimeout = entity.Timeout ?? 0;
                entity.Timeout = (currentTimeout < 0) ? currentTimeout - extendDays : -extendDays;
                entity.DoUser = currentUser;
                entity.DoDate = GetCurrentTime();
            }
            return ResultModel<bool>.Ok(true); // 注意：保存操作在外部处理
        }
        
        #endregion

        #region 5.2. 通用查询构建 (Common Query Builders)

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
        /// 阀体价格查询：应用筛选条件
        /// </summary>
        private static IQueryable<Ask_DataFTDto> ApplyFTFilters(IQueryable<Ask_DataFTDto> query, Ask_DataFTQto qto, bool includeExpiredFilter = true)
        {
            // 使用扩展方法简化筛选逻辑
            query = query.ApplyDateRangeFilter(x => x.AskDate, qto.StartDate, qto.EndDate);
            query = query.ApplyKeywordFilter(x => x.AskProjName, qto.AskProjName);
            query = query.ApplyKeywordFilter(x => x.SuppName, qto.SuppName);

            // 特定筛选条件
            if (qto.BillID.HasValue)
                query = query.Where(x => x.BillID == qto.BillID.Value);

            if (qto.SuppID.HasValue)
                query = query.Where(x => x.SuppID == qto.SuppID.Value);

            if (!string.IsNullOrWhiteSpace(qto.OrdVersion))
                query = query.ApplyKeywordFilter(x => x.OrdVersion, qto.OrdVersion);

            if (!string.IsNullOrWhiteSpace(qto.OrdDN))
                query = query.ApplyKeywordFilter(x => x.OrdDN, qto.OrdDN);

            if (!string.IsNullOrWhiteSpace(qto.OrdPN))
                query = query.ApplyKeywordFilter(x => x.OrdPN, qto.OrdPN);

            if (!string.IsNullOrWhiteSpace(qto.OrdFT))
                query = query.ApplyKeywordFilter(x => x.OrdFT, qto.OrdFT);

            if (qto.FTTypeId.HasValue)
                query = query.Where(x => x.FTTypeId == qto.FTTypeId.Value);

            return query;
        }

        /// <summary>
        /// 附件价格查询：应用筛选条件
        /// </summary>
        private static IQueryable<Ask_DataFJDto> ApplyFJFilters(IQueryable<Ask_DataFJDto> query, Ask_DataFJQto qto, bool includeExpiredFilter = true)
        {
            // 使用扩展方法简化筛选逻辑
            query = query.ApplyDateRangeFilter(x => x.AskDate, qto.StartDate, qto.EndDate);
            query = query.ApplyKeywordFilter(x => x.AskProjName, qto.AskProjName);
            query = query.ApplyKeywordFilter(x => x.SuppName, qto.SuppName);

            // 特定筛选条件
            if (qto.BillID.HasValue)
                query = query.Where(x => x.BillID == qto.BillID.Value);

            if (qto.SuppID.HasValue)
                query = query.Where(x => x.SuppID == qto.SuppID.Value);

            // 附件特有筛选条件
            if (!string.IsNullOrWhiteSpace(qto.FJType))
                query = query.ApplyKeywordFilter(x => x.FJType, qto.FJType);

            if (qto.FJTypeId.HasValue)
                query = query.Where(x => x.FJTypeId == qto.FJTypeId.Value);

            if (!string.IsNullOrWhiteSpace(qto.FJVersion))
                query = query.ApplyKeywordFilter(x => x.FJVersion, qto.FJVersion);

            return query;
        }

        /// <summary>
        /// 构建阀体价格基础查询（联表并投影为 Ask_DataFTDto）
        /// </summary>
        private IQueryable<Ask_DataFTDto> BuildDataFTBaseQuery(IQueryable<IDataFTEntity> dataSource, Dictionary<string, int> ftTypeMapping, int isInvalid)
        {
            var query = from d in dataSource
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
                            FTTypeId = ftTypeMapping.ContainsKey(d.OrdVersion ?? "") ? ftTypeMapping[d.OrdVersion ?? ""] : null,
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
                            IsInvalid = isInvalid,
                                      Timeout = d.Timeout,
                                      ProjDay = d.ProjDay,
                                      Day1 = d.Day1,
                                      Day2 = d.Day2,
                                      Memo1 = d.Memo1,
                                      BillIDText = billDetail != null ? billDetail.BillID.ToString() : null
                                  };

            return query;
        }

        /// <summary>
        /// 构建阀体价格-有效数据查询（联表并投影为 Ask_DataFTDto）
        /// </summary>
        private IQueryable<Ask_DataFTDto> BuildDataFTActiveQuery(Dictionary<string, int> ftTypeMapping)
        {
            return BuildDataFTBaseQuery(_db.Ask_DataFT.AsNoTracking(), ftTypeMapping, 1);
        }

        /// <summary>
        /// 构建阀体价格-过期数据查询（联表并投影为 Ask_DataFTDto）
        /// </summary>
        private IQueryable<Ask_DataFTDto> BuildDataFTExpiredQuery(Dictionary<string, int> ftTypeMapping)
        {
            return BuildDataFTBaseQuery(_db.Set<Ask_DataFTOut>().AsNoTracking(), ftTypeMapping, 0);
        }
        /// <summary>
        /// 构建附件价格查询基础方法（联表并投影为 Ask_DataFJDto）
        /// </summary>
        private IQueryable<Ask_DataFJDto> BuildDataFJBaseQuery(IQueryable<IDataFJEntity> dataSource, Dictionary<string, int> fjTypeMapping, int isInvalid)
        {
            var query = from d in dataSource
                        join billDetail in _db.Ask_BillDetail.AsNoTracking() on d.BillDetailID equals billDetail.ID
                        join bill in _db.Ask_Bill.AsNoTracking() on billDetail.BillID equals bill.BillID
                        join supplier in _db.Ask_Supplier.AsNoTracking() on d.Supplier equals supplier.ID.ToString() into supplierGroup
                                  from supplier in supplierGroup.DefaultIfEmpty()
                                  join bp in (
                            from p in _db.Ask_BillPrice.AsNoTracking()
                                      group p by p.BillDetailID into g
                                      select new { BillDetailID = g.Key, BasicsPrice = g.Max(x => x.BasicsPrice ?? 0), AddPrice = g.Max(x => x.AddPrice ?? 0) }
                                  ) on d.BillDetailID equals bp.BillDetailID into priceGroup
                                  from billPrice in priceGroup.DefaultIfEmpty()
                        join billFile in _db.Ask_BillFile.AsNoTracking() on d.BillDetailID equals billFile.BillDetailID into fileGroup
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
                            FJTypeId = fjTypeMapping.ContainsKey(d.FJType ?? "") ? fjTypeMapping[d.FJType ?? ""] : null, // 从字典映射获取，避免子查询
                                      FJVersion = d.FJVersion,
                                      Num = d.Num,
                                      Unit = d.Unit,
                                      IsPreProBind = d.IsPreProBind,
                                      DocName = bill.DocName,
                                      FileName = billFile != null ? billFile.FileName : null,
                                      Timeout = d.Timeout,
                            IsInvalid = isInvalid,
                                      ProjDay = d.ProjDay,
                                      Day1 = d.Day1,
                                      Day2 = d.Day2,
                                      Memo1 = d.Memo1,
                                      BillIDText = billDetail.BillID.ToString()
                                  };

            return query;
        }

        /// <summary>
        /// 构建附件价格-有效数据查询
        /// </summary>
        private IQueryable<Ask_DataFJDto> BuildDataFJActiveQuery(Dictionary<string, int> fjTypeMapping)
        {
            return BuildDataFJBaseQuery(_db.Ask_DataFJ.AsNoTracking(), fjTypeMapping, 1);
        }

        /// <summary>
        /// 构建附件价格-过期数据查询
        /// </summary>
        private IQueryable<Ask_DataFJDto> BuildDataFJExpiredQuery(Dictionary<string, int> fjTypeMapping)
        {
            return BuildDataFJBaseQuery(_db.Ask_DataFJOut.AsNoTracking(), fjTypeMapping, 0);
        }


        #endregion

        #region 5.3. 通用工具 (Common Utilities)

        /// <summary>
        /// 记录修改日志
        /// </summary>
        private void AddLog(int mainId, string dataType, string? partType, string? partVersion, string? partName, double ratio, string? currentUser)
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
                    CreateDate = GetCurrentTime()
                };

                _db.Ask_FTFJListLog.Add(log);

                _logger.LogInformation("成功添加修改日志: MainID={MainID}, DataType={DataType}, User={User}",
                    mainId, dataType, currentUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "添加修改日志失败: MainID={MainID}, DataType={DataType}, User={User}",
                    mainId, dataType, currentUser);

                // 日志记录是必需的，重新抛出异常
                throw;
            }
        }

        /// <summary>
        /// 创建日志对象（用于批量操作，不保存到数据库）
        /// </summary>
        private Ask_FTFJListLog CreateLog(int mainId, string dataType, string? partType, string? partVersion, string? partName, double ratio, string? currentUser)
        {
            return new Ask_FTFJListLog
            {
                MainID = mainId,
                DataType = dataType,
                PartType = partType,
                PartVersion = partVersion,
                PartName = partName,
                Ratio = ratio,
                CreateUser = currentUser ?? "系统用户",
                CreateDate = GetCurrentTime()
            };
        }

        /// <summary>
        /// 批量设置 Timeout 与 IsPreProBind
        /// </summary>
        private static void SetTimeoutAndBind<T>(IEnumerable<T> items, int timeout, int isPreProBind) where T : class, IDataItemDto
        {
            foreach (var item in items)
            {

                item.Timeout = timeout;
                item.IsPreProBind = isPreProBind;
            }
        }

        /// <summary>
        /// 批量设置 Timeout 与 IsPreProBind 
        /// </summary>
        private static void SetTimeoutAndBindEntities<T>(IEnumerable<T> items, int timeout, int isPreProBind) where T : class, ITimeoutBindable
        {
            foreach (var item in items)
            {
                item.Timeout = timeout;
                item.IsPreProBind = isPreProBind;
            }
        }

        /// <summary>
        /// 获取当前时间（统一时间处理）
        /// </summary>
        private DateTime GetCurrentTime() => _dateTimeProvider.UtcNow;

        /// <summary>
        /// 获取附件类型ID映射字典
        /// </summary>
        private async Task<Dictionary<string, int>> GetFJTypeIdMappingAsync()
        {
            const string cacheKey = "FJ_TYPE_ID_MAPPING";

            if (_cache.TryGetValue(cacheKey, out Dictionary<string, int> mapping))
            {
                return mapping;
            }

            mapping = await _db.Ask_FJList
                .AsNoTracking()
                .ToDictionaryAsync(x => x.FJType ?? "", x => x.ID);

            _cache.Set(cacheKey, mapping, TimeSpan.FromMinutes(30));

            return mapping;
        }

        /// <summary>
        /// 获取阀体类型ID映射字典（带缓存）
        /// </summary>
        private async Task<Dictionary<string, int>> GetFTTypeIdMappingAsync()
        {
            const string cacheKey = "FT_TYPE_ID_MAPPING";

            if (_cache.TryGetValue(cacheKey, out Dictionary<string, int> mapping))
            {
                return mapping;
            }

            mapping = await _db.Ask_FTList
                .AsNoTracking()
                .Select(t => new { t.FTVersion, t.ID })
                .ToDictionaryAsync(t => t.FTVersion ?? "", t => t.ID);

            _cache.Set(cacheKey, mapping, TimeSpan.FromMinutes(30));

            return mapping;
        }

        /// <summary>
        /// 警告信息
        /// </summary>
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

        /// <summary>
        /// 验证采购成本字段填写规则
        /// </summary>
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
        /// 装饰价格查询结果的派生显示字段
        /// </summary>
        private void DecorateDataList<T>(IEnumerable<T> items) where T : class, IDataItemDto
        {
            foreach (var item in items)
            {
                item.IsPreProBindText = ZKLT25Profile.GetPreProBindText(item.IsPreProBind);

                var isInvalid = item.IsInvalid;
                var isValid = isInvalid == 1;

                item.PriceStatusText = isValid ? "有效" : "已过期";
                item.AvailableActions = isValid ? "延长有效期,设置过期" : "设置有效";
                item.DocNameStatus = GetUploadStatusText(item.DocName);
                item.FileNameStatus = GetUploadStatusText(item.FileName);
            }
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
        /// 根据文件名返回上传状态文本
        /// </summary>
        private static string GetUploadStatusText(string? fileName)
        {
            return string.IsNullOrWhiteSpace(fileName) ? "未上传" : "下载";
        }
        
        // --- Excel Helpers ---

        /// <summary>
        /// 通用Excel数据导入方法
        /// </summary>
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
        /// 验证上传的Excel文件
        /// </summary>
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
        private Worksheet ReadExcelFile(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var workbook = new Workbook(stream);
            return workbook.Worksheets[0];
        }

        /// <summary>
        /// 通用Excel文件创建方法
        /// </summary>
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

        #endregion
    }
}