using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TodoContext _dbContext;

        public UserRepository(TodoContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> CreateAsync(User newUser)
        {
            newUser.UniqueId = Guid.NewGuid();            
            
            User user = new User

            {
                UniqueId = newUser.UniqueId,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Age = newUser.Age,
                Birthday = newUser.Birthday,
                Address = newUser.Address,
                Employments = newUser.Employments
            };

            _dbContext.Users.Add(user);

            await _dbContext.SaveChangesAsync();

            return await GetAsync(user.Id);

        }

        public async Task DeleteAsync(Guid userGuid)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.UniqueId == userGuid)
                .SingleOrDefaultAsync();

            if (user == null)
            {
                return;
            }

            _dbContext.Users.Remove(user);

            await _dbContext.SaveChangesAsync();

        }

        public async Task<ActionResult<IEnumerable<User>>> FindAsync()
        {
            return await _dbContext.Users
                     .AsNoTracking()
                     .Include(a => a.Address)
                     .Include(e => e.Employments)
                     .ToListAsync();           
        }

        public async Task<User?> GetAsync(int id)
        {


            User? user = await _dbContext.Users
                .AsNoTracking()
                .Where(e => e.Id == id)
                .SingleOrDefaultAsync();

            if (user == null)
            {
                return null;
            }

            return user;
        }

        public async Task<User?> GetByGuidAsync(Guid guid)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .Include(a => a.Address)
                .Include(e => e.Employments)
                .Where(u => u.UniqueId == guid)
                .SingleOrDefaultAsync();

            if (user == null)
            {
                return null;
            }

            return user;
        }

        public async Task<User?> UpdateAsync(User updatedUser)
        {

            User? user = await _dbContext.Users               
                .Include(a =>a.Address)
                .Include(e => e.Employments)
                .Where(e => e.Id == updatedUser.Id)
                .SingleOrDefaultAsync();


            if (user == null)
            {
                return null;
            }

            //THIS IS WORKING
            //THIS IS WORKING
            //THIS IS WORKING
            //THIS IS WORKING
            //THIS IS WORKING

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Birthday = updatedUser.Birthday;

            //THIS IS NOT WORKING
            //THIS IS NOT WORKING
            //THIS IS NOT WORKING
            //THIS IS NOT WORKING

            user.Address!.Street = string.IsNullOrEmpty(updatedUser.Address?.Street) ? null : updatedUser.Address.Street;
            user.Address!.City = updatedUser.Address!.City;
            user.Address!.PostCode = updatedUser.Address.PostCode;


            foreach (Employment employment in user.Employments) //take a look, we are using user.Employments
            {
                //Employment add
                if (employment.Id == 0) //if is 0 is a new one, thus we are adding
                {
                    user.Employments?.Add(employment);
                    continue; // 
                }

                //Update Employment that is already existing
                Employment? employmentEntity = user.Employments?.FirstOrDefault(x => x.Id == employment.Id);

                //Seems this is failing
                if (employmentEntity == null)
                {
                    continue;
                }

                employmentEntity.StartDate = employment.StartDate;
                employmentEntity.EndDate = employment.EndDate;
                employmentEntity.Company = employment.Company;
                employmentEntity.Salary = employment.Salary;
            }

            /*_dbContext.Entry(user).State = EntityState.Modified;*/

            await _dbContext.SaveChangesAsync();

            return await GetAsync(updatedUser.Id);
        }
    }
}
