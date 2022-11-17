using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TodoApi.Middleware;
using TodoApi.Models;
using TodoApi.Repositories;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using TodoApi.Exceptions;
using ValidationException = TodoApi.Exceptions.ValidationException;
using System.Net;

namespace TodoApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserRepository _userRepository;
        //private readonly IEmploymentRepository _employmentRepository;

        public UsersController(IUserRepository userRepository, ILogger<UsersController> logger)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        
        [HttpGet]       
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            var user = await _userRepository.FindAsync();

            if (user == null)
            {
                throw new Exception("notFound");
            }

            return Ok(user);
        }

        // GET: api/Users/Guid
        [HttpGet("{userGuid}")]
        public async Task<ActionResult<User>> GetUser(string userGuid)
        {
            Guid guid = Guid.TryParse(userGuid, out Guid parsedGuid) ? parsedGuid : Guid.Empty;

            var user = await _userRepository.GetByGuidAsync(guid);

            if (user == null)
            {
                //return NotFound();
                throw new NotFoundException();
            }

            return user;
        }

        // PUT: api/Users/5
        [HttpPut("{userGuid}")]
        public async Task<ActionResult<User>> PutUser(string userGuid, [FromBody] User user) // we use from body when we use information from the payload
        {
            //FIRST THING TO DO: VALIDATIONS
            //USER VALIDATION
            if (string.IsNullOrEmpty(user.FirstName))
            {
                throw new ValidationException(nameof(user.FirstName), "Mandatory");
            }

            if (string.IsNullOrEmpty(user.LastName))
            {
                throw new ValidationException(nameof(user.LastName), "Mandatory");
            }

            if (user.Age == null)
            {              
                throw new ValidationException(nameof(user.Age), "Mandatory");
            }

            if (user.Birthday == null)
            {
                throw new ValidationException(nameof(user.Birthday), "Mandatory");
            }

            //ADDRESS VALIDATIONS
            if (string.IsNullOrEmpty(user.Address?.Street))
            {
                throw new ValidationException(nameof(user.Address.Street), "Mandatory");
            }

            if (string.IsNullOrEmpty(user.Address.City))
            {
                throw new ValidationException(nameof(user.Address.City), "Mandatory");
            }

            //EMPLOYEE VALIDATION
            var query = user.Employments?.GroupBy(x => x.Company)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .Count();

            if (query > 0)
            {
                throw new ValidationException(nameof(Employment.Company), "Mandatory");
            }

            //ONLY NEED ONE FOREACH
            foreach (var employment in user.Employments!)
            {
                if (employment.Salary == null)
                {
                    throw new ValidationException(nameof(employment.Salary), "Mandatory");
                }
                if (employment.StartDate == null)
                {
                    throw new ValidationException(nameof(employment.StartDate), "Mandatory");
                }
            }

            //SECOND PART. AFTER CHECKING EVERYTHING IS OK, IT IS TIME TO SAVE DATA
            Guid guid = Guid.TryParse(userGuid, out Guid parsedGuid) ? parsedGuid : Guid.Empty;


            var userEntity = await _userRepository.GetByGuidAsync(guid);            

            if (userEntity == null)
            {
                return NotFound();
            }

            //UPDATE USER ENTITY
            // This is need to updated
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

            User? updatedUser = await _userRepository.UpdateAsync(userEntity); //SAVING*/            

            if (updatedUser == null)
            {
                return NotFound();
            }

            return updatedUser;
        }


        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            //CREATING GUIDs
            user.UniqueId = Guid.NewGuid();

            //VALIDATIONS
            if (string.IsNullOrEmpty(user.FirstName))
            {
                throw new ValidationException(nameof(user.FirstName), "Mandatory");
            }

            if (string.IsNullOrEmpty(user.LastName))
            {
                throw new ValidationException(nameof(user.LastName), "Mandatory");
            }

            if (user.Age == null)
            {
                throw new ValidationException(nameof(user.Age), "Mandatory");
            }

            if (user.Birthday == null)
            {
                throw new ValidationException(nameof(user.Birthday), "Mandatory");
            }

            //ADDRESS VALIDATIONS
            if (string.IsNullOrEmpty(user.Address?.Street))
            {
                throw new ValidationException(nameof(user.Address.Street), "Mandatory");
            }

            if (string.IsNullOrEmpty(user.Address.City))
            {
                throw new ValidationException(nameof(user.Address.City), "Mandatory");
            }

            //COMPANY VALIDATION
            var query = user.Employments.GroupBy(x => x.Company)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .Count();

            if (query > 0)
            {
                throw new ValidationException(nameof(Employment.Company), "Mandatory");
            }

            //ONLY NEED ONE FOREACH
            foreach (var employment in user.Employments)
            {

                if (employment.Salary == null)
                {
                    throw new ValidationException(nameof(Employment.Salary), "Mandatory");
                }

                if (employment.StartDate == null)
                {
                    throw new ValidationException(nameof(Employment.StartDate), "Mandatory");
                }
            }
            
            User? createdUser = await _userRepository.CreateAsync(user);
            
            if (createdUser == null)
            {
                return NotFound();
            }

            return Ok(createdUser);
        }
                
        [HttpDelete("{userGuid}")]
        public async Task<IActionResult> DeleteUser(Guid userGuid)
        {
            User? user = await _userRepository.GetByGuidAsync(userGuid);
            
            if (user == null)
            {

                return NotFound();

            }

            await _userRepository.DeleteAsync(userGuid);          

            return NoContent();
        }
        
    }
}
