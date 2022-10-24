using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TodoApi.Models;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using HttpPutAttribute = Microsoft.AspNetCore.Mvc.HttpPutAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace TodoApi.Repositories
{
    
    public interface IUserRepository
    {   
          /*  > GetAsync(int id);
            > GetByGuidAsync(Guid guid);
            > CreateAsync(User newUser);
            > UpdateAsync(User updatedUser);
            > DeleteAsync(int id);*/
        public Task<ActionResult<IEnumerable<User>>> GetAsync();
        public Task<ActionResult<User>> GetByGuidAsync(string userGuid);

        public Task<ActionResult<User>> CreateAsync(User user);

        public Task<ActionResult<User>> UpdateAsync(string userGuid, [FromBody] User user); // we use from body when we use information from the payload

        public Task<IActionResult> DeleteAsync(string userGuid);

    }
}
