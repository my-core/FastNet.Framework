using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FastNet.Framework.JwtAuthorize
{
    /// <summary>
    /// jwt声明，用于获取JwtUser
    /// </summary>
    public class JwtClaimed
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private JwtOptions _jwtOptions;
        public JwtClaimed(IHttpContextAccessor httpContextAccessor,JwtOptions jwtOptions)
        {
            _httpContextAccessor = httpContextAccessor;
            _jwtOptions = jwtOptions;
        }

        /// <summary>
        /// 认证用户信息
        /// </summary>
        public JwtUser JwtUser
        {
            get
            {
                var authenticationUser = _httpContextAccessor.HttpContext.User;
                if (authenticationUser != null && authenticationUser.Identity.IsAuthenticated)
                {
                    //var userData = authenticationUser.FindFirst(claim => claim.Type == ClaimTypes.UserData).Value;
                    //return JsonConvert.DeserializeObject<JwtUser>(userData);
                    var userId = authenticationUser.FindFirst(claim => claim.Type == ClaimTypes.Name).Value;
                    return new JwtUser
                    {
                        UserID = Convert.ToInt64(userId)
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 创建JwtToken,默认有效期7天
        /// </summary>
        /// <param name="jwtUser"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public string CreateJwtToken(JwtUser jwtUser)
        {
            return CreateJwtToken(jwtUser, TimeSpan.FromDays(7));
        }

        /// <summary>
        /// 创建JwtToken,指定有效期
        /// </summary>
        /// <param name="jwtUser"></param>
        /// <param name="secret">对HmacSha256Signature方式，密钥长度不应小于128位,也就是至少有16个字符</param>
        /// <param name="expires"></param>
        /// <returns></returns>
        public string CreateJwtToken(JwtUser jwtUser, TimeSpan expires)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var identity = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, jwtUser.UserID.ToString()),
                    //序列化JwtUser对象
                    //new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(jwtUser))
                });
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.Issuer,
                Audience=_jwtOptions.Audience,
                Subject = identity,
                Expires = DateTime.Now.Add(expires),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)), SecurityAlgorithms.HmacSha256)
            };
            //签发一个加密后的用户信息凭证，用来标识用户的身份
            //_httpContextAccessor.HttpContext.SignInAsync(JwtBearerDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            //生成token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
