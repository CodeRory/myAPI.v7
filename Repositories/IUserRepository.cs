using TodoApi.Models;

namespace TodoApi.Repositories
{
    public interface IUserRepository
    {
        Task<IReadOnlyCollection<User>> FindAsync(int userId);
        Task<User?> GetAsync(int id);
        Task<User?> GetByGuidAsync(string guid);
        Task<User?> CreateAsync(User newUser);
        Task<User?> UpdateAsync(User updatedUser);
        Task DeleteAsync(string userGuid);
    }
}
