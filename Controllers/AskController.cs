using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using ZKLT25.API.Helper;
using ZKLT25.API.IServices;
using ZKLT25.API.IServices.Dtos;

namespace ZKLT25.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class AskController : ControllerBase
    {
        private readonly IAskService _service;

        public AskController(IAskService service)
        {
            _service = service;
        }

        #region 阀体型号维护

        /// <summary>
        /// 分页查询阀体型号列表
        /// </summary>
        /// <param name="qto">查询参数</param>
        /// <returns></returns>
        [HttpGet("GetFTPagedList")]
        public async Task<ResultModel<PaginationList<Ask_FTListDto>>> GetPagedListAsync([FromQuery] Ask_FTListQto qto)
        {
            var res = await _service.GetFTPagedListAsync(qto);
            if (res.Data != null)
                Response.Headers.Append("TotalCount", res.Data.TotalCount.ToString());
            return res;
        }

        /// <summary>
        /// 新增阀体型号
        /// </summary>
        /// <param name="cto">创建参数</param>
        /// <returns></returns>
        [HttpPost("CreateFT")]
        public async Task<ResultModel<bool>> CreateFTAsync([FromBody] Ask_FTListCto cto)
        {
            return await _service.CreateFTAsync(cto);
        }

        /// <summary>
        /// 更新阀体型号
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="uto">更新参数</param>
        /// <returns></returns>
        [HttpPut("{id}/UpdateFT")]
        public async Task<ResultModel<bool>> UpdateFTAsync([FromRoute] int id, [FromBody] Ask_FTListUto uto)
        {
            var currentUser = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return await _service.UpdateFTAsync(id, uto, currentUser);
        }

        /// <summary>
        /// 删除阀体型号
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        [HttpDelete("{id}/DeleteFT")]
        public async Task<ResultModel<bool>> DeleteFTAsync([FromRoute] int id)
        {
            return await _service.DeleteFTAsync(id);
        }

        /// <summary>
        /// 检查阀体型号是否存在
        /// </summary>
        /// <param name="ftVersion">阀体型号</param>
        /// <param name="excludeId">排除的ID（编辑时用）</param>
        /// <returns></returns>
        [HttpGet("CheckFTExists")]
        public async Task<ResultModel<bool>> CheckFTExistsAsync([FromQuery] string ftVersion, [FromQuery] int? excludeId = null)
        {
            return await _service.ExistsFTAsync(ftVersion, excludeId);
        }

        /// <summary>
        /// 批量更新阀体系数
        /// </summary>
        [HttpPost("BatchUpdateFTRatio")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ResultModel<bool>> BatchUpdateFTRatioAsync([FromBody] BatchUpdateRatioRequest request)
        {
            var currentUser = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return await _service.BatchUpdateFTRatioAsync(request.Ids, request.Ratio, currentUser);
        }

        #endregion

        #region 附件型号维护

        /// <summary>
        /// 分页查询附件列表
        /// </summary>
        /// <param name="qto">查询参数</param>
        /// <returns></returns>
        [HttpGet("GetFJPagedList")]
        public async Task<ResultModel<PaginationList<Ask_FJListDto>>> GetFJPagedListAsync([FromQuery] Ask_FJListQto qto)
        {
            var res = await _service.GetFJPagedListAsync(qto);
            if (res.Data != null)
                Response.Headers.Append("TotalCount", res.Data.TotalCount.ToString());
            return res;
        }

        /// <summary>
        /// 批量更新附件系数
        /// </summary>
        [HttpPost("BatchUpdateFJRatio")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ResultModel<bool>> BatchUpdateFJRatioAsync([FromBody] BatchUpdateRatioRequest request)
        {
            var currentUser = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return await _service.BatchUpdateFJRatioAsync(request.Ids, request.Ratio, currentUser);
        }

        #endregion

        #region 阀体/附件日志维护

        /// <summary>
        /// 分页查询阀体/附件日志
        /// </summary>
        /// <param name="qto">查询参数，需指定MainID和DataType</param>
        /// <returns>分页日志列表</returns>
        [HttpGet("GetFTFJLogPagedList")]
        public async Task<ResultModel<PaginationList<Ask_FTFJListLogDto>>> GetFTFJLogPagedListAsync([FromQuery] Ask_FTFJListLogQto qto)
        {
            var res = await _service.GetFTFJLogPagedListAsync(qto);
            if (res.Data != null)
                Response.Headers.Append("TotalCount", res.Data.TotalCount.ToString());
            return res;
        }

        #endregion

        #region 供应商维护

        /// <summary>
        /// 分页查询供应商列表
        /// </summary>
        /// <param name="qto">查询参数</param>
        /// <returns></returns>
        [HttpGet("GetSPPagedList")]
        public async Task<ResultModel<PaginationList<Ask_SupplierDto>>> GetPagedListAsync([FromQuery] Ask_SupplierQto qto)
        {
            var res = await _service.GetSPPagedListAsync(qto);
            if (res.Data != null)
                Response.Headers.Append("TotalCount", res.Data.TotalCount.ToString());
            return res;
        }

        /// <summary>
        /// 新增供应商信息
        /// </summary>
        /// <param name="cto">创建参数</param>
        /// <returns></returns>
        [HttpPost("CreateSP")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ResultModel<bool>> CreateSPAsync([FromBody] Ask_SupplierCto cto)
        {
            var currentUser = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return await _service.CreateSPAsync(cto, currentUser);
        }

        /// <summary>
        /// 更新供应商信息
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="uto">更新参数</param>
        /// <returns></returns>
        [HttpPut("{id}/UpdateSP")]
        public async Task<ResultModel<bool>> UpdateSPAsync([FromRoute] int id, [FromBody] Ask_SupplierUto uto)
        {
            var currentUser = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return await _service.UpdateSPAsync(id, uto, currentUser);
        }

        /// <summary>
        /// 删除供应商信息
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        [HttpDelete("{id}/DeleteSP")]
        public async Task<ResultModel<bool>> DeleteSPAsync([FromRoute] int id)
        {
            return await _service.DeleteSPAsync(id);
        }

        /// <summary>
        /// 检查供应商信息是否存在
        /// </summary>
        /// <param name="SuppName">供应商名称</param>
        /// <param name="excludeId">排除的ID（编辑时用）</param>
        /// <returns></returns>
        [HttpGet("CheckSPExists")]
        public async Task<ResultModel<bool>> CheckSPExistsAsync([FromQuery] string SuppName, [FromQuery] int? excludeId = null)
        {
            return await _service.ExistsSPAsync(SuppName, excludeId);
        }

        /// <summary>
        /// 获取供应商附件配置页面
        /// </summary>
        /// <param name="supplierId">供应商ID</param>
        /// <returns></returns>
        [HttpGet("GETSPFJ")]
        public async Task<IActionResult> GetSPFJ(int supplierId)
        {
            var result = await _service.GetSPFJPageAsync(supplierId);
            return Ok(result);
        }

        /// <summary>
        /// 保存供应商附件配置
        /// </summary>
        /// <param name="supplierId">供应商ID</param>
        /// <param name="suppliedFJTypes">选中供应的附件类型列表</param>
        /// <returns></returns>
        [HttpPost("UpdateSPFJ")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> SaveSPFJ(int supplierId, [FromBody] List<string> suppliedFJTypes)
        {
            var result = await _service.BatchUpdateSPFJAsync(supplierId, suppliedFJTypes);
            return Ok(result);
        }

        /// <summary>
        /// 获取供应商阀体配置页面
        /// </summary>
        /// <param name="supplierId">供应商ID</param>
        /// <returns></returns>
        [HttpGet("GETSPFT")]
        public async Task<IActionResult> GetSPFT(int supplierId)
        {
            var result = await _service.GetSPFTPageAsync(supplierId);
            return Ok(result);
        }

        /// <summary>
        /// 保存供应商阀体配置
        /// </summary>
        /// <param name="supplierId">供应商ID</param>
        /// <param name="cto">配置请求</param>
        /// <returns></returns>
        [HttpPost("UpdateSPFT")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> SaveSPFT(int supplierId, [FromBody] BatchUpdateSPFTCto cto)
        {
            var currentUser = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var result = await _service.BatchUpdateSPFTAsync(supplierId, cto.SuppliedFTItems, currentUser);
            return Ok(result);
        }


        #endregion

        #region 询价

        /// <summary>
        /// 分页查询询价列表
        /// </summary>
        /// <param name="qto">查询参数</param>
        /// <returns></returns>
        [HttpGet("GetBillPagedList")]
        public async Task<ResultModel<PaginationList<Ask_BillDto>>> GetBillPagedListAsync([FromQuery] Ask_BillQto qto)
        {
            var res = await _service.GetBillPagedListAsync(qto);
            if (res.Data != null)
                Response.Headers.Append("TotalCount", res.Data.TotalCount.ToString());
            return res;
        }

        /// <summary>
        /// 获取询价详情
        /// </summary>
        /// <param name="billId">账单ID</param>
        /// <returns></returns>
        [HttpGet("GetBillDetails/{billId}")]
        public async Task<ResultModel<List<Ask_BillDetailDto>>> GetBillDetailsAsync(int billId)
        {
            return await _service.GetBillDetailsAsync(billId);
        }

        /// <summary>
        /// 获取询价状态日志
        /// </summary>
        /// <param name="qto">查询参数</param>
        /// <returns></returns>
        [HttpPost("GetBillLogs")]
        public async Task<ResultModel<List<Ask_BillLogDto>>> GetBillLogsAsync([FromBody] Ask_BillLogQto qto)
        {
            return await _service.GetBillLogsAsync(qto);
        }
        #endregion

        #region 阀体附件价格查询
        /// <summary>
        /// 分页查询阀体价格
        /// </summary>
        /// <param name="qto">查询参数</param>
        /// <returns></returns>
        [HttpGet("GetDataFTPagedList")]
        public async Task<ResultModel<PaginationList<Ask_DataFTDto>>> GetDataFTPagedListAsync([FromQuery] Ask_DataFTQto qto)
        {
            var res = await _service.GetDataFTPagedListAsync(qto);
            if (res.Data != null)
                Response.Headers.Append("TotalCount", res.Data.TotalCount.ToString());
            return res;
        }

        /// <summary>
        /// 分页查询附件价格
        /// </summary>
        /// <param name="qto">查询参数</param>
        /// <returns></returns>
        [HttpGet("GetDataFJPagedList")]
        public async Task<ResultModel<PaginationList<Ask_DataFJDto>>> GetDataFJPagedListAsync([FromQuery] Ask_DataFJQto qto)
        {
            var res = await _service.GetDataFJPagedListAsync(qto);
            if (res.Data != null)
                Response.Headers.Append("TotalCount", res.Data.TotalCount.ToString());
            return res;
        }

        /// <summary>
        /// 设置价格状态
        /// </summary>
        /// <param name="request">请求参数</param>
        /// <returns></returns>
        [HttpPost("SetPriceStatus")]
        public async Task<ResultModel<bool>> SetPriceStatusAsync([FromBody] SetPriceStatusRequest request)
        {
            var currentUser = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return await _service.SetPriceStatusAsync(request.Ids, request.Action, request.ExtendDays, currentUser, request.EntityType);
        }
        #endregion

        #region 导出阀体附件价格文件
        /// <summary>
        /// 导出附件询价数据为 Excel 文件
        /// </summary>
        /// <param name="qto">查询条件</param>
        /// <returns>Excel 文件下载</returns>
        [HttpPost("DataFJExcelList")]
        public async Task<IActionResult> DataFJExcelListAsync([FromBody] Ask_DataFJQto qto)
        {
            try
            {
                var excelBytes = await _service.DataFJExcelAsync(qto);
                var fileName = $"附件询价数据_{DateTime.Now:yyyyMMddHHmm}.xlsx";
                
                return File(excelBytes, 
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"导出失败：{ex.Message}" });
            }
        }

        /// <summary>
        /// 导出阀体询价数据为 Excel 文件
        /// </summary>
        /// <param name="qto">查询条件</param>
        /// <returns>Excel 文件下载</returns>
        [HttpPost("DataFTExcelList")]
        public async Task<IActionResult> DataFTExcelListAsync([FromBody] Ask_DataFTQto qto)
        {
            try
            {
                var excelBytes = await _service.DataFTExcelAsync(qto);
                var fileName = $"阀体询价数据_{DateTime.Now:yyyyMMddHHmm}.xlsx";
                return File(excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"导出失败：{ex.Message}" });
            }
        }

        #endregion

        #region 价格备注录入

        /// <summary>
        /// 录入价格备注
        /// </summary>
        [HttpPost("SetPriceRemark")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        public async Task<ResultModel<int>> SetPriceRemarkAsync([FromForm] BillPriceCto cto)
        {
            var currentUser = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            
            // 在控制器层处理文件，避免 IFormFile 渗透到服务层
            if (cto.QuoteFile != null && cto.BillDetailIDs.Any())
            {
                return await _service.SetPriceRemarkAsync(cto, currentUser, cto.QuoteFile.FileName, cto.QuoteFile.OpenReadStream());
            }
            
            return await _service.SetPriceRemarkAsync(cto, currentUser);
        }


        /// <summary>
        /// 关闭项目
        /// </summary>
        [HttpPost("CloseProject")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ResultModel<int>> CloseProjectAsync([FromBody] int billId)
        {
            var currentUser = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return await _service.CloseProjectAsync(billId, currentUser);
        }

        #endregion

        #region 采购成本维护

        /// <summary>
        /// 分页查询采购成本列表
        /// </summary>
        /// <param name="qto">查询参数</param>
        /// <returns></returns>
        [HttpGet("GetCGPagedList")]
        public async Task<ResultModel<PaginationList<Ask_CGPriceValueDto>>> GetCGPagedListAsync([FromQuery] Ask_CGPriceValueQto qto)
        {
            var res = await _service.GetCGPagedListAsync(qto);
            if (res.Data != null)
                Response.Headers.Append("TotalCount", res.Data.TotalCount.ToString());
            return res;
        }

        /// <summary>
        /// 新增采购成本记录
        /// </summary>
        /// <param name="cto">创建参数</param>
        /// <returns></returns>
        [HttpPost("CreateCG")]
        public async Task<ResultModel<bool>> CreateCGAsync([FromBody] Ask_CGPriceValueCto cto)
        {
            return await _service.CreateCGAsync(cto);
        }

        /// <summary>
        /// 删除采购成本记录
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        [HttpDelete("DeleteCG/{id}")]
        public async Task<ResultModel<bool>> DeleteCGAsync(int id)
        {
            return await _service.DeleteCGAsync(id);
        }

        /// <summary>
        /// 更新采购成本记录
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="uto">更新参数</param>
        /// <returns></returns>
        [HttpPut("UpdateCG/{id}")]
        public async Task<ResultModel<bool>> UpdateCGAsync(int id, [FromBody] Ask_CGPriceValueUto uto)
        {
            return await _service.UpdateCGAsync(id, uto);
        }

        /// <summary>
        /// 导出采购成本数据为Excel
        /// </summary>
        /// <param name="qto">查询参数</param>
        /// <returns></returns>
        [HttpPost("ExportCGExcel")]
        public async Task<IActionResult> ExportCGExcelAsync([FromBody] Ask_CGPriceValueQto qto)
        {
            try
            {
                var excelBytes = await _service.ExportCGExcelAsync(qto);
                var fileName = $"采购成本库数据_{DateTime.Now:yyyyMMddHHmm}.xlsx";
                
                return File(excelBytes, 
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"导出失败：{ex.Message}" });
            }
        }

        /// <summary>
        /// 导入采购成本数据Excel
        /// </summary>
        /// <param name="file">Excel文件</param>
        /// <param name="isReplace">是否全量替换</param>
        /// <returns></returns>
        [HttpPost("ImportCGExcel")]
        public async Task<ResultModel<ImportResult>> ImportCGExcelAsync(IFormFile file, [FromQuery] bool isReplace = false)
        {
            return await _service.ImportCGExcelAsync(file, isReplace);
        }

        #endregion
    }
}
