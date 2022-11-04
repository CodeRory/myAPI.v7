using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.SecurityTokenService;
using Newtonsoft.Json;
using SendGrid.Helpers.Errors.Model;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using BadRequestException = Microsoft.IdentityModel.SecurityTokenService.BadRequestException;
using Microsoft.Extensions.Logging;
using static Umbraco.Core.Collections.TopoGraph;
using System.Web.Http;

namespace TodoApi.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class MyMiddleware
    {


        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        //We need in the constructor RequestDelegate in order to make it work
        public MyMiddleware(RequestDelegate next, ILoggerFactory logFactory)
        {
            _next = next;

            _logger = logFactory.CreateLogger("MyMiddleware");
        }


        //WITH THIS METHOD WE ARE GOING TO INVOKE OUR MIDDLEWARE
        public async Task Invoke(HttpContext context, ILogger<MyMiddleware> logger)
        {
            try
            {
                _logger.LogInformation("MyMiddleware executing...");

                await _next(context); // calling next middleware
            }
            catch(Exception ex)
            {
                await HandleExceptionAsync(context, ex, logger);
            }
        }

        //IF SOMETHING FAILS, INVOKE IS GOING TO CALL THIS METHOD, THAT IT IS GOING TO BE USED FOR HANDLE ERRORS. 
        private Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger)
        {
            string result;

            //ONLY IF WITHOUT ELSE BECAUSE IT IS FASTER
            if(exception.Message == "validationException")
            {
                context.Response.WriteAsync("Validation error");
                result = "There is an error in the validation";
                logger.LogInformation(result);
            }

            if(exception is BadRequestException)
            {
                context.Response.WriteAsync("There is an error on the request");
                result = "There is an error on the request";
            }

            if(exception is NotFoundException)
            {
                context.Response.WriteAsync("Not found exception");
                result = "Not found!";
            }

            if(exception is InternalServerErrorResult)
            {
                context.Response.WriteAsync("There is an error");
                result = "Internal server error!";
            }    

            //THIS IS NOT WORKING
            /* switch (exception)
             {
                 case ValidationException validationException:
                     context.Response.WriteAsync("Validation error");
                     result = "There is an error in the validation";
                     logger.LogInformation(result);
                     break;
                 case BadRequestException badRequestException:
                     context.Response.WriteAsync("There is an error on the request");
                     result = "There is an error on the request";
                     break;
                 case NotFoundException notFoundException:
                     context.Response.WriteAsync("Not found exception");
                     result = "Not found!";
                     break;
                 default:
                     context.Response.WriteAsync("There is an error");
                     result = "Internal server error!";
                     break;
             }           */

           // return context.Response.WriteAsync(result);
           return context.Response.WriteAsync("hola");
        }        
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MyMiddleware>();
        }
    }
}
