using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Text; 
using System.Threading.Tasks; 
using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Microsoft.AspNetCore.Builder; 
using Microsoft.AspNetCore.Hosting; 
using Microsoft.AspNetCore.HttpsPolicy; 
using Microsoft.AspNetCore.Mvc; 
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.DependencyInjection; 
using Microsoft.Extensions.Logging; 
using Microsoft.Extensions.Options; 
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using NLog.Web;

namespace dotnetCoreApi {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration; 
        }

        public IConfiguration Configuration {get; }

        public void ConfigureServices(IServiceCollection services) {
            services.Configure < JwtSettings > (Configuration.GetSection("JwtSettings")); 

            var jwtSettings = new JwtSettings(); 
            Configuration.Bind("JwtSettings", jwtSettings); 

            services.AddAuthentication(options =>  {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; 
            })
            .AddJwtBearer(o =>  {
                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters {
                    ValidIssuer = jwtSettings.Issuer, 
                    ValidAudience = jwtSettings.Audience, 
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
　　　　　　　　　　　ValidateIssuerSigningKey = true, 
                    ValidateIssuer = true,
　　　　　　　　　　　//是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
　　　　　　　　　　　ValidateLifetime = true, 
　　　　　　　　　　　//允许的服务器时间偏移量
　　　　　　　　　　　ClockSkew = TimeSpan.Zero

                }; 
            }); 


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1); 
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env,ILoggerFactory loggerFactory) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage(); 
            }
            else {
                app.UseHsts(); 
            }
            loggerFactory.AddNLog(); 
            env.ConfigureNLog("nlog.config");//读取Nlog配置文件
            
            app.UseGlobalException();
            app.UseHttpsRedirection();
            app.UseAuthentication(); 
            app.UseMvc(); 
        }
    }
}
