using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZKLT25.API.Helper;
using ZKLT25.API.IServices;
using ZKLT25.API.IServices.Dtos;

namespace ZKLT25.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
            return await _service.UpdateFTAsync(id, uto);
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
        public async Task<ResultModel<bool>> BatchUpdateFTRatioAsync([FromBody] BatchUpdateRatioRequest request)
        {
            return await _service.BatchUpdateFTRatioAsync(request.Ids, request.Ratio);
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
        public async Task<ResultModel<bool>> BatchUpdateFJRatioAsync([FromBody] BatchUpdateRatioRequest request)
        {
            return await _service.BatchUpdateFJRatioAsync(request.Ids, request.Ratio);
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
        public async Task<ResultModel<bool>> CreateSPAsync([FromBody] Ask_SupplierCto cto)
        {
            return await _service.CreateSPAsync(cto);
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
            return await _service.UpdateSPAsync(id, uto);
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
        [HttpGet("supplier/{supplierId}/fj-config")]
        public async Task<IActionResult> GetSupplierFJConfig(int supplierId)
        {
            var result = await _service.GetSupplierFJConfigPageAsync(supplierId);
            return Ok(result);
        }

        /// <summary>
        /// 保存供应商附件配置
        /// </summary>
        /// <param name="supplierId">供应商ID</param>
        /// <param name="suppliedFJTypes">选中供应的附件类型列表</param>
        /// <returns></returns>
        [HttpPost("supplier/{supplierId}/fj-config")]
        public async Task<IActionResult> SaveSupplierFJConfig(int supplierId, [FromBody] List<string> suppliedFJTypes)
        {
            var result = await _service.BatchUpdateSupplierFJConfigAsync(supplierId, suppliedFJTypes);
            return Ok(result);
        }

        /// <summary>
        /// 获取供应商阀体配置页面
        /// </summary>
        /// <param name="supplierId">供应商ID</param>
        /// <returns></returns>
        [HttpGet("supplier/{supplierId}/ft-config")]
        public async Task<IActionResult> GetSupplierFTConfig(int supplierId)
        {
            var result = await _service.GetSupplierFTConfigPageAsync(supplierId);
            return Ok(result);
        }

        /// <summary>
        /// 保存供应商阀体配置
        /// </summary>
        /// <param name="supplierId">供应商ID</param>
        /// <param name="cto">配置请求</param>
        /// <returns></returns>
        [HttpPost("supplier/{supplierId}/ft-config")]
        public async Task<IActionResult> SaveSupplierFTConfig(int supplierId, [FromBody] BatchUpdateSupplierFTConfigCto cto)
        {
            var result = await _service.BatchUpdateSupplierFTConfigAsync(supplierId, cto.SuppliedFTItems);
            return Ok(result);
        }

        #endregion
    }
}
