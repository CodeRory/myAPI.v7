﻿using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetEmployments(Guid userGuid)
        {
            User? user = await _userRepository.GetByGuidAsync(userGuid);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(await _employmentRepository.FindAsync(user.Id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid userGuid, int id)
        {
            User? user = await _userRepository.GetByGuidAsync(userGuid);
                

            if (user == null)
            {
                return NotFound();
            }

            Employment? employment = (Employment?)await _employmentRepository.FindAsync(id);
                

            if (employment == null)
            {
                return NotFound();
            }

            await _employmentRepository.DeleteAsync(id, employment.Id);

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> PostUser(Guid userGuid, Employment requestEmployment)
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

            User? user = await _userRepository.GetByGuidAsync(userGuid);

            //Creating userId
            requestEmployment.UserId = user!.Id;

            if (user == null)
            {
                return NotFound();
            }

            await _employmentRepository.CreateAsync(requestEmployment);     

            /*await _employmentRepository.SaveChangesAsync();*/

            return Ok(requestEmployment);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> PutUser(Guid userGuid, int id, Employment requestEmployment) // we use from body when we use information from the payload
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
                        

            User? user = await _userRepository.GetByGuidAsync(userGuid);

            if (user == null)
            {
                return NotFound();
            }
            Employment? employment = (Employment?)await _employmentRepository.FindAsync(id);

            if (employment == null)
            {
                return NotFound();
            }

            //NOW IS CREATING PROPERLY BUT NOW CORRECTLY ASSIGNED
            employment.StartDate = requestEmployment.StartDate;
            employment.EndDate = requestEmployment.EndDate;
            employment.Company = requestEmployment.Company;
            employment.Salary = requestEmployment.Salary;

           /* await _dbContext.SaveChangesAsync(); //SAVING*/
            User? resultUser = await _userRepository.GetByGuidAsync(userGuid);

            if (resultUser == null)
            {
                return NotFound();
            }

            return resultUser;
        }


        [HttpGet("current")]
        public async Task<ActionResult<User>> GetCurrentEmployment(Guid userGuid) // we use from body when we use information from the payload
        {
            User? user = await _userRepository.GetByGuidAsync(userGuid);

            if (user == null)
            {
                return NotFound();
            }

            Employment? employment = (Employment?)await _employmentRepository.FindAsync(user.Id);

            if (employment == null)
            {
                return NotFound();
            }

            return Ok(employment);
        }
    }
}