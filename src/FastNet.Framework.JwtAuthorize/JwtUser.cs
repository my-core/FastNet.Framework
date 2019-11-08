using System;
using System.Collections.Generic;
using System.Text;

namespace FastNet.Framework.JwtAuthorize
{
    /// <summary>
    /// jwt token中的用户信息,用于扩展其他数据
    /// </summary>
    public class JwtUser
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public long UserID { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 用户类型
        /// </summary>
        public int UserType { get; set; }
        /// <summary>
        /// 渠道
        /// </summary>
        public int UserChannel { get; set; }
    }
}
