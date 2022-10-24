using Microsoft.EntityFrameworkCore;
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
                Company = newEmployment.Company,
                MonthOfExperince = newEmployment.MonthOfExperince,
                Salary = newEmployment.Salary,
                StartDate = newEmployment.StartDate,
                EndDate = newEmployment.EndDate,
            };

            await _dbContext.SaveChangesAsync();

            return await GetAsync(employment.UserId, employment.Id);
        }

        public async Task DeleteAsync(int userId, int employmentId)
        {
            Employment? employment = await _dbContext.Employments
                .AsNoTracking()
                .Where(e => e.UserId == userId & e.Id == employmentId)
                .SingleOrDefaultAsync();

            if(employment == null)
            {
                return;
            }

            _dbContext.Employments.Remove(employment);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<Employment>> FindAsync()
        {
            return await _dbContext.Employments.ToListAsync();
        }

        public async Task<Employment?> GetAsync(int userId, int employmentId)
        {
            Employment? employment = await _dbContext.Employments
                .AsNoTracking()
                .Where(e => e.UserId == userId && e.Id == employmentId)
                .SingleOrDefaultAsync();

            if(employment == null)
            {
                return null;
            }

            return employment;
        }

        public async Task<Employment?> UpdateAsync(Employment updateEmployment)
        {
            Employment? employment = await _dbContext.Employments
                .AsNoTracking()
                .Where(e => e.UserId == updateEmployment.UserId && e.Id == updateEmployment.Id)
                .SingleOrDefaultAsync();

            if (employment == null)
            {
                return null;
            }

            return employment;

            employment.Company = updateEmployment.Company;
            employment.MonthOfExperince = updateEmployment.MonthOfExperince;
            employment.Salary = updateEmployment.Salary;
            employment.StartDate = updateEmployment.StartDate;
            employment.EndDate = updateEmployment.EndDate;

            await _dbContext.SaveChangesAsync();

            return await GetAsync(employment.UserId, employment.Id);
        }
    }
}
