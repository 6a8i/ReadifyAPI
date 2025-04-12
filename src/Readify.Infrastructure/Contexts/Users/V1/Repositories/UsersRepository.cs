using Microsoft.EntityFrameworkCore;
using Readify.Application.Features.Users.V1.Infrastructure.Entities;
using Readify.Application.Features.Users.V1.Infrastructure.IRepositories;
using Readify.Infrastructure.Commons.DatabaseContexts.V1;

namespace Readify.Infrastructure.Contexts.Users.V1.Repositories
{
    public class UsersRepository(ReadifyDatabaseContext context) : IUsersRepository
    {
        private readonly ReadifyDatabaseContext _context = context;

        public async Task<Guid?> AddAsync(User entity)
        {
            await _context.Users.AddAsync(entity);
            
            var result = await _context.SaveChangesAsync();

            if (result > 0)
                return entity.Id;
            else
                return null;
        }

        public async Task<List<User>> GetAllAsync()
        {
            var result = await _context.Users.ToListAsync();
            return result;
        }

        public async Task<string?> GetPasswordByEmailAsync(Guid id)
        {
            var password = await _context.Users
                                            .Where(u => u.Id == id)
                                            .Select(u => u.Password)
                                            .FirstOrDefaultAsync();
            return password;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var entity = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            return entity;
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            var result = await _context.Users.FirstOrDefaultAsync(b => b.Id == id);
            return result;
        }

        public async Task<bool> UpdateAsync(User entity)
        {
            var result = await _context
                                .Users
                                .Where(b => b.Id == entity.Id)
                                .ExecuteUpdateAsync(b =>
                                    b.SetProperty(p => p.IsActive, v => entity.IsActive)
                                     .SetProperty(p => p.Password, v => entity.Password)
                                     .SetProperty(p => p.Name, v => entity.Name)
                                     .SetProperty(p => p.Email, v => entity.Email));
            return result > 0;
        }
    }
}
