using FluentResults;
using Microsoft.EntityFrameworkCore;
using Readify.Application.Features.Authentications.V1.Infrastructure.Entities;
using Readify.Application.Features.Authentications.V1.Infrastructure.IRepositories;
using Readify.Infrastructure.Commons.DatabaseContexts.V1;

namespace Readify.Infrastructure.Contexts.Authentication.V1.Repositories
{
    public class AuthenticationRepository(ReadifyDatabaseContext context) : IAuthenticationRepository
    {
        private readonly ReadifyDatabaseContext _context = context;

        public async Task<Guid> CreateTokenAsync(Token token)
        {
            _context.Tokens.Add(token);
            int result;
            try
            {
                result = await _context.SaveChangesAsync();
                
                if (result > 0)
                    return token.Id;
                else
                    return Guid.Empty;
            }
            catch (Exception)
            {
                return Guid.Empty;
            }
        }

        public async Task<bool> ExpiresAllTokensByUserAsync(Guid userId)
        {
            var result = await _context.Tokens
                                            .Where(t => t.UserId == userId && !t.HasExpired)
                                            .ExecuteUpdateAsync(t => 
                                                t.SetProperty(p => p.HasExpired, true));
            result = await _context.Tokens
                                            .Where(t => t.UserId == userId && t.ExpiresAt > DateTime.UtcNow)
                                            .ExecuteUpdateAsync(t => 
                                                t.SetProperty(p => p.ExpiresAt, DateTime.UtcNow.AddHours(-1)));

            return result > 0;
        }

        public async Task<Token?> GetTokenByIdAsync(Guid id)
        {
            var result = await _context.Tokens.FindAsync(id);

            return result;
        }

        public async Task<Token?> GetTokenByUserIdAsync(Guid id)
        {
            var result = await _context.Tokens.OrderByDescending(t => t.ExpiresAt).FirstOrDefaultAsync(x => x.UserId == id && !x.HasExpired);
            return result;
        }

        public async Task<bool> UpdateTokenAsync(Token value)
        {
            _context.Update(value);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}
