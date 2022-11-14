
using Microsoft.IdentityModel.Protocols.WSTrust;
using System.Net;
using System.Web.Mvc;
using TodoApi.Exceptions;
using ValidationException = TodoApi.Exceptions.ValidationException;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;


namespace TodoApi.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
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

        //LET'S TRY THIS CLASS
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public class NotFoundWithMessageResult
        {
            private string message;

            public NotFoundWithMessageResult(string message)
            {
                this.message = message;
            }

            public Task<HttpResponseMessage> ExecuteError()
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                response.Content = new StringContent(message);
                return Task.FromResult(response);
            }
        }

        NotFoundWithMessageResult NotFoundWithMessageRes = new NotFoundWithMessageResult("Validation error");

        //IF SOMETHING FAILS, INVOKE IS GOING TO CALL THIS METHOD, THAT IT IS GOING TO BE USED FOR HANDLE ERRORS. 
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        private Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger)
        {
            
            switch (exception)
            {

                ///USE VALIDATION ERROR AND RETURN A BADREQUEST STATUS
                case ValidationException validationException:
                    context.Response.WriteAsync("Validation error");
                    //NotFoundWithMessageRes.ExecuteError();
                    BadRequestResult result = new BadRequestResult();
                    return BadRequest(ModelState);
                    //return BadRequest();
                    break;
                case NotFoundException notFoundException:
                    context.Response.WriteAsync("Not found exception");
                    break;
                default:
                    context.Response.WriteAsync("There is an error");
                    break;
            }

            return context.Response.WriteAsync("\nThis is a Middleware message");
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
