using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;
using TodoApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Umbraco.Core.Persistence;

namespace TodoApi.Middleware
{
    public class ExceptionsHandlingMiddleware
    {
        
        
        private string BuildResponse(string message)
        {
            return JsonConvert.SerializeObject(new { GeneralErrors = new[] { message } });
        }

        private string BuildMetadataResponse(string message)
        {
            return JsonConvert.SerializeObject(new { ErrorMessage = message });
        }

    }
}
