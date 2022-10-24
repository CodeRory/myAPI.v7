using TodoApi.Models;

namespace TodoApi.Repositories
{
    public interface IUserRepository
    {
        Task<IReadOnlyCollection<Employment>> FindAsync(int userId);
        Task<Employment?> GetAsync(int id);
        Task<Employment?> GetByGuidAsync(Guid guid);
        Task<Employment?> CreateAsync(User newUser);
        Task<Employment?> UpdateAsync(User updatedUser);
        Task DeleteAsync(int id);
    }
}
