using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json; 
using System; 
using System.Collections.Generic; 
using System.IO; 
using System.Linq; 
using System.Net; 
using System.Threading.Tasks; 
using System.Xml.Serialization; 

namespace dotnetCoreApi {
    public class GlobalExceptionMiddleware {
        private  readonly RequestDelegate next;
        private  readonly ILogger _logger;

        public GlobalExceptionMiddleware(RequestDelegate next,ILogger<GlobalExceptionMiddleware> logger) {
            this.next = next; 
            this._logger=logger;
        }

        public async Task Invoke(HttpContext context) {
            try {
                await next(context); 
            }
            catch (Exception ex) {
                await HandleExceptionAsync(context, ex); 
            }
        }

        private  async Task HandleExceptionAsync(HttpContext context, Exception exception) {
            if (exception == null)return; 
            await WriteExceptionAsync(context, exception).ConfigureAwait(false); 
        }

        private  async Task WriteExceptionAsync(HttpContext context, Exception exception) {
            //记录日志
              _logger.LogError($"系统发生未处理异常：{exception.StackTrace}");
            //返回友好的提示
            var response = context.Response; 

            //状态码
            if (exception is UnauthorizedAccessException)
                response.StatusCode = (int)HttpStatusCode.Unauthorized; 
            else if (exception is Exception)
                response.StatusCode = (int)HttpStatusCode.BadRequest; 

            response.ContentType = context.Request.Headers["Accept"];

            response.ContentType = "application/json"; 
            await response.WriteAsync(JsonConvert.SerializeObject(new {state=400,message="出现未知异常"})).ConfigureAwait(false); 
        }

    }

     public static class VisitLogMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalException(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}