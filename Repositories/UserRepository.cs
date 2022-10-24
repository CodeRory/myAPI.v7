using TodoApi.Models;

namespace TodoApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        public Task<Employment?> CreateAsync(User newUser)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<Employment>> FindAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<Employment?> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Employment?> GetByGuidAsync(Guid guid)
        {
            throw new NotImplementedException();
        }

        public Task<Employment?> UpdateAsync(User updatedUser)
        {
            throw new NotImplementedException();
        }
    }
}
