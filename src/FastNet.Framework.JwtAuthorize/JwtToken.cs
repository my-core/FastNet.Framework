using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FastNet.Framework.JwtAuthorize
{
    public class JwtToken
    {
        /// <summary>
        /// 创建JwtToken,默认有效期7天
        /// </summary>
        /// <param name="jwtUser"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static string CreateJwtToken(JwtUser jwtUser, string secret)
        {
            return CreateJwtToken(jwtUser, secret, TimeSpan.FromDays(7));
        }
        /// <summary>
        /// 创建JwtToken,指定有效期
        /// </summary>
        /// <param name="jwtUser"></param>
        /// <param name="secret"></param>
        /// <param name="expires"></param>
        /// <returns></returns>
        public static string CreateJwtToken(JwtUser jwtUser, string secret, TimeSpan expires)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, jwtUser.UserID.ToString()),
                    //序列化JwtUser对象
                    new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(jwtUser))
                }),
                Expires = DateTime.Now.Add(expires),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        
    }
}
