using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FastNet.Framework.JwtAuthorize;

namespace Test.JwtAuthorize
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private JwtClaimed _jwtClaimed;
        public UserController(JwtClaimed jwtClaimed)
        {
            _jwtClaimed = jwtClaimed;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet("token")]
        [AllowAnonymous]
        public string GetToken()
        {

            return _jwtClaimed.CreateJwtToken(new JwtUser
            {
                UserID = 1,
                NickName = "test",
                UserChannel = 1,
                UserType = 1
            });
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet("info")]
        public JwtUser GetUser()
        {
            return _jwtClaimed.JwtUser;
        }
    }
}
