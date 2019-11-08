using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public JwtClaimed(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
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
                    var userData = authenticationUser.FindFirst(claim => claim.Type == ClaimTypes.UserData).Value;
                    return JsonConvert.DeserializeObject<JwtUser>(userData);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
