using Microsoft.AspNetCore.Diagnostics;

namespace ZKLT25.API.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class BizExceptionHandler(IWebHostEnvironment environment) : IExceptionHandler
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
            if (exception is not BizException bizException)
            {
                return false;
            }

            var res = ResultModel<string>.Error(bizException.Message);

            httpContext.Response.StatusCode = StatusCodes.Status200OK;
            await httpContext.Response.WriteAsJsonAsync(res, cancellationToken);
            return true;
        }
    }
}
