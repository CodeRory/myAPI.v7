using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Extensions;
using TodoApi.Models;

namespace TodoApi.Repositories
{
    public interface IEmploymentRepository
    {

        public Task<IActionResult> GetAsync(string userGuid);

        public Task<ActionResult<User>> UpdateAsync(string userGuid, int id, Employment requestEmployment); // we use from body when we use information from the payload

        public Task<IActionResult> CreateAsync(string userGuid, Employment requestEmployment);

        [HttpDelete("{id}")]
        public Task<IActionResult> DeleteAsync(string userGuid, int id);              
    }
}
