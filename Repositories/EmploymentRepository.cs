using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task<Employment?> CreateAsync(Employment newEmployment)
        {
           
            // save
            Employment employment = new Employment
            {
                UserId = newEmployment.UserId,
                Company = newEmployment.Company,
                MonthOfExperince = newEmployment.MonthOfExperince,
                Salary = newEmployment.Salary,
                StartDate = newEmployment.StartDate,
                EndDate = newEmployment.EndDate,
            };

            _dbContext.Employments.Add(employment);

            await _dbContext.SaveChangesAsync();

            return await GetAsync(employment.UserId, employment.Id);
            
        }

        public async Task<Employment?> DeleteAsync(int userId, int id)
        {

            var user = await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .SingleOrDefaultAsync();

            if (user == null)
            {
                return null;
            }

            Employment? employment = await _dbContext.Employments
               .AsNoTracking()
               .Where(e => e.UserId == user.Id && e.Id == id)
               .SingleOrDefaultAsync();

            if (employment == null)
            {
                return null;
            }

            _dbContext.Employments.Remove(employment);

            await _dbContext.SaveChangesAsync();

            return employment;
        }

        public async Task<IReadOnlyCollection<Employment>> FindAsyncGuid(Guid guid)
        {
            User? user = await _dbContext.Users
                .AsNoTracking()
                .Include(u => u.Address)
                .Include(e => e.Employments)
                .SingleOrDefaultAsync(u => u.UniqueId == guid);

            if (user == null)
            {
                return new List<Employment>();
            }

            return await _dbContext.Employments
                .AsNoTracking()
                .Where(e => e.UserId == user.Id)
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<Employment>> FindAsyncId(int id)

        {
            User? user = await _dbContext.Users
                .AsNoTracking()
                .Include(u => u.Address)
                .Include(e => e.Employments)
                .SingleOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return new List<Employment>();
            }

            return await _dbContext.Employments
                .AsNoTracking()
                .Where(e => e.UserId == user.Id)
                .ToListAsync();
        }


        public async Task<Employment?> GetCurrentEmploymentAsync(Guid guid)
        {
            User? user = await _dbContext.Users
                .AsNoTracking()
                .Include(u => u.Address)
                .Include(e => e.Employments)
                .SingleOrDefaultAsync(u => u.UniqueId == guid);

            if (user == null)
            {
                return null;
            }

            return await _dbContext.Employments
                .AsNoTracking()
                .Where(e => e.UserId == user.Id)
                .OrderByDescending(e => e.StartDate)
                .FirstOrDefaultAsync();
        }

        public async Task<Employment?> GetAsync(int userId/*this is for user*/, int id)
        {            

            Employment? employment = await _dbContext.Employments
                .AsNoTracking()
                .Where(e => e.UserId == userId && e.Id == id)
                .SingleOrDefaultAsync();

            if (employment == null)
            {
                return null;
            }

            return employment;
        }

        public async Task<Employment?> UpdateAsync(Employment updateEmployment)
        {
            Employment? employment = await _dbContext.Employments
                //.AsNoTracking()
                .Where(e => e.UserId == updateEmployment.UserId && e.Id == updateEmployment.Id)
                .SingleOrDefaultAsync();

            if (employment == null)
            {
                return null;
            }

            //return employment;

            employment.Company = updateEmployment.Company;
            employment.MonthOfExperince = updateEmployment.MonthOfExperince;
            employment.Salary = updateEmployment.Salary;
            employment.StartDate = updateEmployment.StartDate;
            employment.EndDate = updateEmployment.EndDate;

            _dbContext.Entry(employment).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();

            /*return await GetAsync(employment.UserId, employment.Id);*/

            return employment;
        }
    }
}
