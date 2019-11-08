using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FastNet.Framework.JwtAuthorize
{
    /// <summary>
    /// jwt认证扩展
    /// </summary>
    public static class JwtAuthorizeExtensions
    {
        /// <summary>
        /// 添加jwt认证服务(默认配置key:"JwtOptions")
        /// </summary>
        /// <param name="services"></param>
        /// <param name="jwtOptions"></param>
        public static void AddJwtAuthorize(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();
            AddJwtAuthorize(services, jwtOptions);
        }
        /// <summary>
        /// 添加jwt认证服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="jwtOptions"></param>
        public static void AddJwtAuthorize(this IServiceCollection services,JwtOptions jwtOptions)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    LifetimeValidator = (before, expires, token, param) =>
                     {
                         var validateResult = expires > DateTime.UtcNow;
                         return validateResult;
                     },
                    ValidateAudience = false,                   
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    //验证签名key
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.Secret))
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        return Task.FromResult(new UnauthorizedResult());
                    }
                };
            });
            //注册JwtClaimed类,在需要使用的地方注入即可
            services.AddSingleton<JwtClaimed>();
        }
    }
}
