using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.IdentityModel.Protocols.WSTrust;
using System.Net;
using System.Web.Http.Results;

namespace TodoApi.Exceptions
{
    public class ValidationException : Exception
    {       

        public ValidationException(string name, string message)
            : base($"There is an error in {name}. This field is {message}.")
        {
 
            
        }
        
    }
}
