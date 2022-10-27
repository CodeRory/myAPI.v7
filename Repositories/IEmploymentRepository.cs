using TodoApi.Models;

namespace TodoApi.Repositories
{
    public interface IEmploymentRepository
    {
        Task<IReadOnlyCollection<Employment>> FindAsync(Guid guid);
        Task<Employment?> FindAsyncCurrent(Guid guid);
        Task<Employment?> GetAsync(int userId, int employmentId);
        Task<Employment?> CreateAsync(Employment newEmployment);
        Task<Employment?> UpdateAsync(Employment updateEmployment);
        Task<Employment?> DeleteAsync(int userId, int employmentId);
    }
}
