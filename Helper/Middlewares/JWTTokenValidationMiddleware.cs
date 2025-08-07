using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using ZKLT25.API.Helper.TokenModule;

namespace ZKLT25.API.Helper.Middlewares
{
    public class JWTTokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _secretKey;
        public JWTTokenValidationMiddleware(RequestDelegate next, IOptions<JwtTokenModel> jwtConfig)
        {
            _next = next;
            _secretKey = jwtConfig.Value.Security;
        }

        public async Task Invoke(HttpContext context)
        {
            if (IsAnonymous(context))
            {
                await _next(context);
                return;
            }

            if (context.Request.Headers.TryGetValue("Authorization", out var authHeader) && authHeader.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.ToString().Substring("Bearer ".Length).Trim();
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = System.Text.Encoding.UTF8.GetBytes(_secretKey);
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    }, out var validatedToken);

                    var jwtToken = (JwtSecurityToken)validatedToken;
                    var externalUserField = jwtToken.Claims.FirstOrDefault(c => c.Type == "IsCustomer")?.Value;
                    if (externalUserField == "1")
                    {
                        context.Response.StatusCode = 403;
                        await context.Response.WriteAsync("外部用户无法访问");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("无效的JWT令牌");
                    return;
                }

            }
            else
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("未提供有效的JWT令牌");
                return;
            }

            await _next(context);
        }

        private bool IsAnonymous(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null)
            {
                return false;
            }

            if (endpoint.Metadata.GetMetadata<AllowAnonymousAttribute>() != null)
            {
                return true;
            }

            var routeData = context.GetRouteData();
            if (routeData != null)
            {
                var actionDescriptor = routeData.Values["actionDescriptor"] as ControllerActionDescriptor;
                if (actionDescriptor != null)
                {
                    return actionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                      .OfType<AllowAnonymousAttribute>().Any() ||
                        actionDescriptor.ControllerTypeInfo.GetCustomAttributes(inherit: true)
                      .OfType<AllowAnonymousAttribute>().Any();
                }
            }

            return false;
        }
    }
}
