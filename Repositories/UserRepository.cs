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
using HttpDeleteAttribute = Microsoft.AspNetCore.Mvc.HttpDeleteAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using HttpPutAttribute = Microsoft.AspNetCore.Mvc.HttpPutAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace TodoApi.Repositories
{
    [Route("api/repositories")]
    [ApiController]
    public class UserRepository : IUserRepository
    {
        private readonly TodoContext _dbContext;        
        public UserRepository(TodoContext dbContext)
        {
            _dbContext = dbContext;
        }


        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAsync()
        {
            //_context is database
            var user = await _dbContext.Users
                .Include(a => a.Address)
                .Include(e => e.Employments)
                .ToListAsync();

            return user;
        }

        // GET: api/Users/Guid
        [HttpGet("{userGuid}")]
        public async Task<ActionResult<User>> GetByGuidAsync(string userGuid)
        {
            Guid guid = Guid.TryParse(userGuid, out Guid parsedGuid) ? parsedGuid : Guid.Empty;

            var user = await _dbContext.UserRepository
                .Include(a => a.Address)
                .Include(e => e.Employments)
                .Where(u => u.UniqueId == guid)
                .SingleOrDefaultAsync();

            //WHY I CANT USE THIS?
           if (user == null)
            {
                return NotFound();
            }

            return user;
        }


        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            //CREATING GUIDs
            user.UniqueId = Guid.NewGuid();

            //VALIDATIONS
            if (string.IsNullOrEmpty(user.FirstName))
            {
                ModelState.AddModelError(nameof(user.FirstName), "First Name is mandatory");
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(user.LastName))
            {
                ModelState.AddModelError(nameof(user.LastName), "Last Name is mandatory");
                return BadRequest(ModelState);
            }

            if (user.Age == null)
            {
                ModelState.AddModelError(nameof(user.Age), "Age is mandatory");
                return BadRequest(ModelState);
            }

            if (user.Birthday == null)
            {
                ModelState.AddModelError(nameof(user.Birthday), "Birthday is mandatory");
                return BadRequest(ModelState);
            }

            //ADDRESS VALIDATIONS
            if (string.IsNullOrEmpty(user.Address?.Street))
            {
                ModelState.AddModelError(nameof(user.Address.Street), "Street is mandatory");
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(user.Address.City))
            {
                ModelState.AddModelError(nameof(user.Address.City), "City is mandatory");
                return BadRequest(ModelState);
            }

            //COMPANY VALIDATION
            var query = user.Employments.GroupBy(x => x.Company)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .Count();

            if (query > 0)
            {
                ModelState.AddModelError(nameof(Employment.Company), "Company must be unique");
                return BadRequest(ModelState);
            }

            //ONLY NEED ONE FOREACH
            foreach (var employment in user.Employments)
            {
                if (employment.Salary == null)
                {
                    ModelState.AddModelError(nameof(employment.Salary), "Salary is mandatory");
                    return BadRequest(ModelState);
                }
                if (employment.StartDate == null)
                {
                    ModelState.AddModelError(nameof(employment.Salary), "Start date is mandatory");
                    return BadRequest(ModelState);
                }
            }

            //AFTER VALIDATIONS, CREATE USER!
            _dbContext.Users.Add(user);

            await _dbContext.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }



        // PUT: api/Users/5
        [HttpPut("{userGuid}")]
        public async Task<ActionResult<User>> PutUser(string userGuid, [FromBody] User user) // we use from body when we use information from the payload
        {
            //FIRST THING TO DO: VALIDATIONS
            //USER VALIDATION
            if (string.IsNullOrEmpty(user.FirstName))
            {
                ModelState.AddModelError(nameof(user.FirstName), "First Name is mandatory");
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(user.LastName))
            {
                ModelState.AddModelError(nameof(user.LastName), "Last Name is mandatory");
                return BadRequest(ModelState);
            }

            if (user.Age == null)
            {
                ModelState.AddModelError(nameof(user.Age), "Age is mandatory");
                return BadRequest(ModelState);
            }

            if (user.Birthday == null)
            {
                ModelState.AddModelError(nameof(user.Birthday), "Birthday is mandatory");
                return BadRequest(ModelState);
            }

            //ADDRESS VALIDATIONS
            if (string.IsNullOrEmpty(user.Address?.Street))
            {
                ModelState.AddModelError(nameof(user.Address.Street), "Street is mandatory");
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(user.Address.City))
            {
                ModelState.AddModelError(nameof(user.Address.City), "City is mandatory");
                return BadRequest(ModelState);
            }

            //EMPLOYEE VALIDATION
            var query = user.Employments?.GroupBy(x => x.Company)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .Count();

            if (query > 0)
            {
                ModelState.AddModelError(nameof(Employment.Company), "Company must be unique");
                return BadRequest(ModelState);
            }

            //ONLY NEED ONE FOREACH
            foreach (var employment in user.Employments!)
            {
                if (employment.Salary == null)
                {
                    ModelState.AddModelError(nameof(employment.Salary), "Salary is mandatory");
                    return BadRequest(ModelState);
                }
                if (employment.StartDate == null)
                {
                    ModelState.AddModelError(nameof(employment.Salary), "Start date is mandatory");
                    return BadRequest(ModelState);
                }
            }

            //SECOND PART. AFTER CHECKING EVERYTHING IS OK, IT IS TIME TO SAVE DATA
            Guid guid = Guid.TryParse(userGuid, out Guid parsedGuid) ? parsedGuid : Guid.Empty;

            User? userEntity = await _dbContext.Users
                .Include(a => a.Address) // Join users
                .Include(e => e.Employments) // Join users
                .Where(u => u.UniqueId == guid)
                .SingleOrDefaultAsync();

            if (userEntity == null)
            {
                return NotFound();
            }

            //UPDATE USER ENTITY
            userEntity.FirstName = user.FirstName;
            userEntity.LastName = user.LastName;
            userEntity.Age = user.Age;
            userEntity.Birthday = user.Birthday;

            userEntity.Address!.Street = string.IsNullOrEmpty(user.Address.Street) ? null : user.Address.Street;  // ! = musn't be null 
            userEntity.Address!.City = user.Address.City;
            userEntity.Address!.PostCode = user.Address.PostCode;

            foreach (Employment employment in user.Employments) //take a look, we are using user.Employments
            {
                //Employment add
                if (employment.Id == 0) //if is 0 is a new one, thus we are adding
                {
                    userEntity.Employments?.Add(employment);
                    continue; // 
                }

                //Update Employment that is already existing
                Employment? employmentEntity = userEntity.Employments?.FirstOrDefault(x => x.Id == employment.Id);

                //Seems this is failing
                if (employmentEntity == null)
                {
                    continue;
                }

                //NOW IS CREATING PROPERLY BUT NOW CORRECTLY ASSIGNED
                employmentEntity.StartDate = employment.StartDate;
                employmentEntity.EndDate = employment.EndDate;
                employmentEntity.Company = employment.Company;
                employmentEntity.Salary = employment.Salary;
            }

            //Remove employment that is not existing on the payload to DB
            string output = JsonConvert.SerializeObject(userEntity.Employments);
            List<Employment> employments = JsonConvert.DeserializeObject<List<Employment>>(output); // came from payload

            foreach (Employment item in employments)
            {
                //Check against the payload
                Employment? employment = user.Employments.FirstOrDefault(e => e.Id == item.Id);

                if (employment == null)
                {
                    //Check against database object
                    Employment? e = userEntity.Employments.FirstOrDefault(u => u.Id == item.Id);

                    if (e != null)
                    {
                        userEntity.Employments.Remove(e);
                    }
                }
            }

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

        [HttpDelete("{userGuid}")]
        public async Task<IActionResult> DeleteUser(string userGuid)
        {
            Guid guid = Guid.TryParse(userGuid, out Guid parsedGuid) ? parsedGuid : Guid.Empty;

            //We have to change our DELETE query
            var user = await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.UniqueId == guid)
                .SingleOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _dbContext.Users.Any(e => e.Id == id);
        }
    }
}
