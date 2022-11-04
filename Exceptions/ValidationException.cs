using System;
using System.Collections.Generic;
using System.Linq;


namespace TodoApi.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
           
        }

    }
}
