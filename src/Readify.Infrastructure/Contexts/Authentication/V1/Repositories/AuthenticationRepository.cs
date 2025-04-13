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

        public async Task<Token?> GetTokenByIdAsync(Guid id)
        {
            var result = await _context.Tokens.FindAsync(id);

            return result;
        }

        public async Task<Token?> GetTokenByUserIdAsync(Guid id)
        {
            var result = await _context.Tokens.FirstOrDefaultAsync(x => x.UserId == id);
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
