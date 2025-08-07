using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ZKLT25.API.Helper.TokenModule
{
    public class TokenHelper
    {
        public static string CreateToken(JwtTokenModel jwtTokenModel)
        {
            //1.定义主体，明文，随token携带
            var claims = new[]
            {
                new Claim("UserId",jwtTokenModel.UserId.ToString()),
                new Claim("Leader",jwtTokenModel.Leader.ToString()),
                new Claim("IsManager",jwtTokenModel.IsManager.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub,jwtTokenModel.UserName),
                new Claim("DepartId",jwtTokenModel.DepartId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("dep", jwtTokenModel.DepartName.ToString()),
                new Claim("IsCustomer",jwtTokenModel.IsCustomer.ToString())
            };
            //2.生成密钥
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenModel.Security));
            //3.通过密钥，生成签名
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //4.通过主体 + 签名 => 生成token
            var token = new JwtSecurityToken(
                issuer: jwtTokenModel.Issuer,
                audience: jwtTokenModel.Audience,
                expires: DateTime.Now.AddDays(jwtTokenModel.Expires),
                signingCredentials: creds,
                claims: claims
            );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return accessToken;
        }
    }
}
