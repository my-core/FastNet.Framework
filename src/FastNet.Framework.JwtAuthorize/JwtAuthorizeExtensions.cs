using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

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
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,//是否验证Issuer
                    ValidateAudience = true,//是否验证Audience
                    ValidateLifetime = true,//是否验证失效时间
                    ValidateIssuerSigningKey = true,//是否验证SecurityKey
                    ValidAudience = jwtOptions.Audience,//Audience
                    ValidIssuer = jwtOptions.Issuer,//Issuer，这两项和前面签发jwt的设置一致
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))//拿到SecurityKey
                };
            });
            //httpcontext
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            //注册jwt配置
            services.AddSingleton(jwtOptions);
            //注册JwtClaimed类,在需要使用的地方注入即可
            services.AddTransient<JwtClaimed>();
        }
    }
}
