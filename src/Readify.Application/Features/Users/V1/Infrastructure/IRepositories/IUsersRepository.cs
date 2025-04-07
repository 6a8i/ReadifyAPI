using Readify.Application.Features.Users.V1.Infrastructure.Entities;

namespace Readify.Application.Features.Users.V1.Infrastructure.IRepositories
{
    public interface IUsersRepository
    {
        Task<Guid?> AddAsync(User entity);
        Task<List<User>> GetAllAsync();
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(Guid id);
        Task<bool> UpdateAsync(User entity);
    }
}
