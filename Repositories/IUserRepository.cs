using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;

namespace TodoApi.Repositories
{
    public interface IUserRepository
    {

        Task<ActionResult<IEnumerable<User>>> FindAsync();
        Task<User?> GetAsync(int id);
        Task<User?> GetByGuidAsync(Guid guid);
        Task<User?> CreateAsync(User newUser);
        Task<User?> UpdateAsync(User updatedUser);
        Task DeleteAsync(Guid userGuid);
    }
}
