using System.Text;
using Microsoft.AspNetCore.Diagnostics;

namespace ZKLT25.API.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class SysExceptionHandler(IWebHostEnvironment environment, ILogger<SysExceptionHandler> logger) : IExceptionHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="exception"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not Exception sysException)
            {
                return false;
            }

            var res = ResultModel<string>.Error(sysException.Message);

            var address = string.Empty;
            try
            {
                var key = "at Microsoft.AspNetCore.Mvc.Infrastructure.ActionMethodExecutor";
                var excs = sysException.StackTrace?.Split(key)[0].Split(" at ").Where(x => !string.IsNullOrEmpty(x.Trim())).ToList() ?? new List<string>();

                excs.Reverse();
                address = string.Join("=>", excs);
            }
            catch
            {
                address = sysException.StackTrace;
            }

            // 读取请求体
            var requestBody = string.Empty;
            try
            {
                if (httpContext.Request.ContentLength > 0 && httpContext.Request.Method == HttpMethod.Post.Method)
                {
                    // 重置请求体流位置，以便可以读取（需在请求管道早期设置 AllowSynchronousIO = true）
                    httpContext.Request.Body.Position = 0;

                    using (var reader = new StreamReader(httpContext.Request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 8192, leaveOpen: true))
                    {
                        requestBody = await reader.ReadToEndAsync();
                    }
                }
            }
            catch { }

            res.Data = $"错误位置: {address}";

            logger.LogError($"\n 错误信息：{res.Message} \n {res.Data} \n 请求参数：{requestBody};");

            httpContext.Response.StatusCode = StatusCodes.Status200OK;
            await httpContext.Response.WriteAsJsonAsync(res, cancellationToken);
            return true;
        }
    }
}
