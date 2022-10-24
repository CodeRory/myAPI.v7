using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Extensions;
using TodoApi.Models;

namespace TodoApi.Repositories
{
    public class EmploymentRepository : IEmploymentRepository
    {

        private readonly TodoContext _dbContext;

        public EmploymentRepository(TodoContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(string userGuid)
        {
            User? user = await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.UniqueId == userGuid.ToGuid())
                .SingleOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            List<Employment> employments = await _dbContext.Employments
                .AsNoTracking()
                .Where(e => e.UserId == user.Id)
                .ToListAsync();

            return Ok(employments);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(string userGuid, Employment requestEmployment)
        {

            //VALIDATIONS
            if (string.IsNullOrEmpty(requestEmployment.Company))
            {
                ModelState.AddModelError(nameof(requestEmployment.Company), "Company is mandatory");
                return BadRequest(ModelState);
            }

            if (requestEmployment.Salary == null)
            {
                ModelState.AddModelError(nameof(requestEmployment.Salary), "Salary is mandatory");
                return BadRequest(ModelState);
            }

            if (requestEmployment.StartDate == null)
            {
                ModelState.AddModelError(nameof(requestEmployment.StartDate), "Start date is mandatory");
                return BadRequest(ModelState);
            }

            if (requestEmployment.StartDate > requestEmployment.EndDate)
            {
                ModelState.AddModelError(nameof(requestEmployment.StartDate), "Start date is wrong");
                return BadRequest(ModelState);
            }

            User? user = await _dbContext.Users
                .AsNoTracking()
                .Include(a => a.Address)
                .Include(e => e.Employments)
                .Where(u => u.UniqueId == userGuid.ToGuid())
                .SingleOrDefaultAsync();

            //Creating userId
            requestEmployment.UserId = user!.Id;

            if (user == null)
            {
                return NotFound();
            }

            //NOT NECCESARY//
            /*//Creating a new instance of Employments (and use the id that we found on line 93)  */
            List<Employment> employments = await _dbContext.Employments
                 .AsNoTracking()
                 .Where(e => e.UserId == user.Id)
                 .ToListAsync();

            if (user == null)
            {
                return NotFound();
            }

            _dbContext.Employments.Add(requestEmployment);

            await _dbContext.SaveChangesAsync();

            return Ok(employments);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateAsync(string userGuid, int id, Employment requestEmployment) // we use from body when we use information from the payload
        {
            //VALIDATIONS
            if (string.IsNullOrEmpty(requestEmployment.Company))
            {
                ModelState.AddModelError(nameof(requestEmployment.Company), "Company is mandatory");
                return BadRequest(ModelState);
            }

            if (requestEmployment.Salary == null)
            {
                ModelState.AddModelError(nameof(requestEmployment.Salary), "Salary is mandatory");
                return BadRequest(ModelState);
            }

            if (requestEmployment.StartDate == null)
            {
                ModelState.AddModelError(nameof(requestEmployment.StartDate), "Start date is mandatory");
                return BadRequest(ModelState);
            }

            if (requestEmployment.StartDate > requestEmployment.EndDate)
            {
                ModelState.AddModelError(nameof(requestEmployment.StartDate), "Start date is wrong");
                return BadRequest(ModelState);
            }


            Guid guid = Guid.TryParse(userGuid, out Guid parsedGuid) ? parsedGuid : Guid.Empty;

            User? user = await _dbContext.Users
               .Where(u => u.UniqueId == userGuid.ToGuid())
               .SingleOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            Employment? employment = await _dbContext.Employments
                .Where(e => e.UserId == user.Id && e.Id == id)
                .SingleOrDefaultAsync();

            if (employment == null)
            {
                return NotFound();
            }

            //NOW IS CREATING PROPERLY BUT NOW CORRECTLY ASSIGNED
            employment.StartDate = requestEmployment.StartDate;
            employment.EndDate = requestEmployment.EndDate;
            employment.Company = requestEmployment.Company;
            employment.Salary = requestEmployment.Salary;

            await _dbContext.SaveChangesAsync(); //SAVING

            User? resultUser = await _dbContext.Users // SELECT
                .AsNoTracking()
                .Include(a => a.Address)
                .Include(e => e.Employments)
                .Where(u => u.UniqueId == guid)
                .SingleOrDefaultAsync();

            if (resultUser == null)
            {
                return NotFound();
            }

            return resultUser;
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string userGuid, int id)
        {
            User? user = await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.UniqueId == userGuid.ToGuid())
                .SingleOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            Employment? employment = await _dbContext.Employments
                .AsNoTracking()
                .Where(e => e.UserId == user.Id && e.Id == id)
                .SingleOrDefaultAsync();

            if (employment == null)
            {
                return NotFound();
            }

            _dbContext.Employments.Remove(employment);

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }



    }
}
