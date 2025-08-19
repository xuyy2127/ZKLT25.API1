using ZKLT25.API.Helper;
using ZKLT25.API.IServices.Dtos;

namespace ZKLT25.API.IServices
{
    public interface IAskService
    {
        #region 阀体型号服务接口
        /// <summary>
        /// 分页查询阀体型号列表
        /// </summary>
        /// <param name="qto">查询条件</param>
        /// <returns></returns>
        Task<ResultModel<PaginationList<Ask_FTListDto>>> GetFTPagedListAsync(Ask_FTListQto qto);

        /// <summary>
        /// 创建阀体型号
        /// </summary>
        /// <param name="cto">创建请求</param>
        /// <returns></returns>
        Task<ResultModel<bool>> CreateFTAsync(Ask_FTListCto cto);

        /// <summary>
        /// 更新阀体型号
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="uto">更新请求</param>
        /// <param name="currentUser">当前用户</param>
        /// <returns></returns>
        Task<ResultModel<bool>> UpdateFTAsync(int id, Ask_FTListUto uto, string? currentUser);

        /// <summary>
        /// 删除阀体型号
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        Task<ResultModel<bool>> DeleteFTAsync(int id);

        /// <summary>
        /// 检查阀体型号是否存在
        /// </summary>
        /// <param name="ftVersion">阀体型号</param>
        /// <param name="excludeId">排除的ID(用于编辑时检查)</param>
        /// <returns></returns>
        Task<ResultModel<bool>> ExistsFTAsync(string ftVersion, int? excludeId = null);

        /// <summary>
        /// 批量更新系数
        /// </summary>
        /// <param name="ids">要更新的记录ID列表</param>
        /// <param name="ratio">新的系数值</param>
        /// <param name="currentUser">当前用户</param>
        /// <returns></returns>
        Task<ResultModel<bool>> BatchUpdateFTRatioAsync(List<int> ids, double ratio, string? currentUser);
        #endregion

        #region 附件服务接口
        /// <summary>
        /// 分页查询附件列表
        /// </summary>
        /// <param name="qto">查询条件</param>
        /// <returns></returns>
        Task<ResultModel<PaginationList<Ask_FJListDto>>> GetFJPagedListAsync(Ask_FJListQto qto);

        /// <summary>
        /// 批量更新系数
        /// </summary>
        /// <param name="ids">要更新的记录ID列表</param>
        /// <param name="ratio">新的系数值</param>
        /// <param name="currentUser">当前用户</param>
        /// <returns></returns>
        Task<ResultModel<bool>> BatchUpdateFJRatioAsync(List<int> ids, double ratio, string? currentUser);
        #endregion

        #region 阀体 / 附件日志服务接口
        /// <summary>
        /// 分页查询阀体 / 附件日志
        /// </summary>
        /// <param name="qto">查询条件（MainID 和 DataType ）</param>
        /// <returns>分页日志列表</returns>
        Task<ResultModel<PaginationList<Ask_FTFJListLogDto>>> GetFTFJLogPagedListAsync(Ask_FTFJListLogQto qto);
        #endregion

        #region 供应商维护服务接口
        /// <summary>
        /// 分页查询供应商列表
        /// </summary>
        /// <param name="qto">查询条件</param>
        /// <returns></returns>
        Task<ResultModel<PaginationList<Ask_SupplierDto>>> GetSPPagedListAsync(Ask_SupplierQto qto);

        /// <summary>
        /// 新建供应商
        /// </summary>
        /// <param name="cto">创建请求</param>
        /// <param name="currentUser">当前用户</param>
        /// <returns></returns>
        Task<ResultModel<bool>> CreateSPAsync(Ask_SupplierCto cto, string? currentUser);

        /// <summary>
        /// 更新供应商信息
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="uto">更新请求</param>
        /// <param name="currentUser">当前用户</param>
        /// <returns></returns>
        Task<ResultModel<bool>> UpdateSPAsync(int id, Ask_SupplierUto uto, string? currentUser);

        /// <summary>
        /// 删除供应商
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        Task<ResultModel<bool>> DeleteSPAsync(int id);

        /// <summary>
        /// 检查供应商是否存在
        /// </summary>
        /// <param name="SuppName">供应商名称</param>
        /// <param name="excludeId">排除的ID(用于编辑时检查)</param>
        /// <returns></returns>
        Task<ResultModel<bool>> ExistsSPAsync(string SuppName, int? excludeId = null);

        /// <summary>
        /// 获取供应商附件配置页面数据
        /// </summary>
        /// <param name="supplierId">供应商ID</param>
        /// <returns></returns>
        Task<ResultModel<List<SPFJPageDto>>> GetSPFJPageAsync(int supplierId);

        /// <summary>
        /// 批量更新供应商附件配置
        /// </summary>
        /// <param name="supplierId">供应商ID</param>
        /// <param name="suppliedFJTypes">选中供应的附件类型列表</param>
        /// <returns></returns>
        Task<ResultModel<bool>> BatchUpdateSPFJAsync(int supplierId, List<string> suppliedFJTypes);

        /// <summary>
        /// 获取供应商阀体配置页面数据
        /// </summary>
        /// <param name="supplierId">供应商ID</param>
        /// <returns></returns>
        Task<ResultModel<List<SPFTPageDto>>> GetSPFTPageAsync(int supplierId);

        /// <summary>
        /// 批量更新供应商阀体配置
        /// </summary>
        /// <param name="supplierId">供应商ID</param>
        /// <param name="suppliedFTItems">选中的阀体配置列表</param>
        /// <param name="currentUser">当前用户</param>
        /// <returns></returns>
        Task<ResultModel<bool>> BatchUpdateSPFTAsync(int supplierId, List<SPFTItem> suppliedFTItems, string? currentUser);

        #endregion

        #region 询价详情
        /// <summary>
        /// 分页查询询价单据
        /// </summary>
        /// <param name="qto">查询条件</param>
        /// <returns></returns>
        Task<ResultModel<PaginationList<Ask_BillDto>>> GetBillPagedListAsync(Ask_BillQto qto);

        /// <summary>
        ///查看询价单据详情
        /// </summary>
        /// <param name="qto">查询条件</param>
        /// <returns></returns>
        Task<ResultModel<List<Ask_BillDetailDto>>> GetBillDetailsAsync(string billId);

        /// <summary>
        /// 获取询价状态日志
        /// </summary>
        Task<ResultModel<List<Ask_BillLogDto>>> GetBillLogsAsync(Ask_BillLogQto qto);
        #endregion

        #region 阀体附件价格查询
        /// <summary>
        /// 分页查询阀体价格
        /// </summary>
        /// <param name="qto">查询条件</param>
        /// <returns></returns>
        Task<ResultModel<PaginationList<Ask_DataFTDto>>> GetDataFTPagedListAsync(Ask_DataFTQto qto);

        /// <summary>
        /// 分页查询附件价格
        /// </summary>
        /// <param name="qto">查询条件</param>
        /// <returns></returns>
        Task<ResultModel<PaginationList<Ask_DataFJDto>>> GetDataFJPagedListAsync(Ask_DataFJQto qto);

        /// <summary>
        /// 设置价格状态
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <param name="action">操作类型：SetValid(设置有效)、SetExpired(设置过期)、ExtendValid(延长有效期)</param>
        /// <param name="extendDays">延长天数（仅在ExtendValid时需要）</param>
        /// <param name="currentUser">当前用户</param>
        /// <param name="entityType">实体类型：DataFT 或 DataFJ</param>
        /// <returns></returns>
        Task<ResultModel<bool>> SetPriceStatusAsync(int id, string action, int? extendDays, string? currentUser, string entityType = "DataFT");
        #endregion

        #region 导出
        /// <summary>
        /// 导出附件询价数据为 Excel 文件
        /// </summary>
        /// <param name="qto">查询条件</param>
        /// <returns>Excel 文件字节数组</returns>
        Task<byte[]> DataFJExcelAsync(Ask_DataFJQto qto);
        #endregion

        #region 价格备注录入接口

        /// <summary>
        /// 录入价格备注
        /// </summary>
        /// <param name="cto">价格录入请求</param>
        /// <param name="currentUser">当前用户</param>
        /// <returns>返回影响的记录数</returns>
        Task<ResultModel<int>> SetPriceRemarkAsync(BillPriceCto cto, string? currentUser);

        /// <summary>
        /// 关闭项目（将状态从发起0改为已关闭-1）
        /// </summary>
        /// <param name="billDetailIds">要关闭的明细ID列表</param>
        /// <param name="currentUser">当前用户</param>
        /// <returns>操作结果</returns>
        Task<ResultModel<int>> CloseProjectAsync(List<int> billDetailIds, string? currentUser);

        #endregion

    }
}