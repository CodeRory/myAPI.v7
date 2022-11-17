
using Microsoft.IdentityModel.Protocols.WSTrust;
using System.Net;
using System.Web.Mvc;
using TodoApi.Exceptions;
using ValidationException = TodoApi.Exceptions.ValidationException;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Umbraco.Core;

namespace TodoApi.Middleware
{
    public class MyMiddleware
    {

        private readonly RequestDelegate _next;

        //We need in the constructor RequestDelegate in order to make it work
        public MyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        //WITH THIS METHOD WE ARE GOING TO INVOKE OUR MIDDLEWARE
        public async Task Invoke(HttpContext context, ILogger<MyMiddleware> logger)
        {
            try
            {
                await _next(context); // calling next middleware
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, logger);
            }
        }  

        private static string ComposeMessage(string message)
        {
            return JsonConvert.SerializeObject(new { Message = message });
        }

        //IF SOMETHING FAILS, INVOKE IS GOING TO CALL THIS METHOD, THAT IT IS GOING TO BE USED FOR HANDLE ERRORS.         
        private Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger)
        {
            
            switch (exception)
            {                
                case ValidationException validationException:                   
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return context.Response.WriteAsync(ComposeMessage(validationException.Message));  
                    
                case NotFoundException notFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return context.Response.WriteAsync("Not found exception");
                   
                default:
                    return context.Response.WriteAsync("There is an error");                   
            }

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
