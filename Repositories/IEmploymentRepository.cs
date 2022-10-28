using TodoApi.Models;

namespace TodoApi.Repositories
{
    public interface IEmploymentRepository
    {
        Task<IReadOnlyCollection<Employment>> FindAsyncGuid(Guid guid);
        Task<IReadOnlyCollection<Employment>> FindAsyncId(int id);
        Task<Employment?> FindAsyncCurrent(Guid guid);
        Task<Employment?> GetAsync(int id/*this is from employee*/, int userId /*this is from employee (1)*/);
        Task<Employment?> CreateAsync(Employment newEmployment);
        Task<Employment?> UpdateAsync(Employment updateEmployment);
        Task<Employment?> DeleteAsync(int userId, int employmentId);
    }
}
