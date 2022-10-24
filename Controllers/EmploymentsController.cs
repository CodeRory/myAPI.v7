using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Execution;
using Microsoft.EntityFrameworkCore;
using TodoApi.Extensions;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Controllers
{
    [Route("api/users/{userGuid}/employments")]
    [ApiController]
    public class EmploymentsController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmploymentRepository _employmentRepository;
       
        public EmploymentsController(
            IUserRepository userRepository,
            EmploymentRepository employmentRepository)
        {
            _employmentRepository = employmentRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployments(string userGuid)
        {
            User? user = _userRepository.GetByGuidAsync(userGuid.ToGuid());

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

        [HttpPost]
        public async Task<IActionResult> PostUser(string userGuid, Employment requestEmployment)
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
        public async Task<ActionResult<User>> PutUser(string userGuid, int id, Employment requestEmployment) // we use from body when we use information from the payload
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


        [HttpGet("current")]
        public async Task<ActionResult<User>> GetCurrentEmployment(string userGuid) // we use from body when we use information from the payload
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
                .Where(e => e.UserId == user.Id)
                .OrderByDescending(e => e.StartDate)
                .FirstOrDefaultAsync();

            if (employment == null)
            {
                return NotFound();
            }

            return Ok(employment);
        }
    }
}
