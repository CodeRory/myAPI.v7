﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        //private readonly IEmploymentRepository _employmentRepository;

        public UsersController(
            IUserRepository userRepository/*,*/
            /*EmploymentRepository employmentRepository*/)
        {
            //_employmentRepository = employmentRepository;
            _userRepository = userRepository;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            User? user = (User?)await _userRepository.FindAsync();

            if (user == null)
            {
                return NotFound();
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
                return NotFound();
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


            var userEntity = await _userRepository.GetByGuidAsync(guid);            

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

            /*await _context.SaveChangesAsync(); //SAVING*/

            var resultUser = await _userRepository.GetByGuidAsync(guid);          

            if (resultUser == null)
            {
                return NotFound();
            }

            return resultUser;
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

            
            await _userRepository.CreateAsync(user);

            return Ok(user);
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
