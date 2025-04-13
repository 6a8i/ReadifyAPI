using FluentResults;
using Readify.Application.Features.Authentications.V1.Infrastructure.Entities;

namespace Readify.Application.Features.Authentications.V1.Infrastructure.IRepositories
{
    public interface IAuthenticationRepository
    {
        Task<Guid> CreateTokenAsync(Token token);
        Task<bool> ExpiresAllTokensByUserAsync(Guid userId);
        Task<Token?> GetTokenByIdAsync(Guid id);
        Task<Token?> GetTokenByUserIdAsync(Guid id);
        Task<bool> UpdateTokenAsync(Token value);
    }
}
