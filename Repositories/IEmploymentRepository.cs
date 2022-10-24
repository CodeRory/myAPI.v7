using TodoApi.Models;

namespace TodoApi.Repositories
{
    public interface IEmploymentRepository
    {
        Task<IReadOnlyCollection<Employment>> FindAsync();
        Task<Employment?> GetAsync(int userId, int employmentId);
        Task<Employment?> CreateAsync(Employment newEmployment);
        Task<Employment?> UpdateAsync(Employment updateEmployment);
        Task DeleteAsync(int userId, int employmentId); 
    }
}
