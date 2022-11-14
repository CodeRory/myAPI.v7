using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.IdentityModel.Protocols.WSTrust;
using System.Net;
using BadHttpRequestException = Microsoft.AspNetCore.Http.BadHttpRequestException;

namespace TodoApi.Exceptions
{
    public class ValidationException : Exception
    {       

        public ValidationException(/*HttpResponse badRequest*/)
            : base("One or more validation failures have occurred.")
        {
            /*this.statusCode = statusCode;*/
        }

        
    }
}
