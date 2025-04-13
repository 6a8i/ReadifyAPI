using FluentResults;
using Readify.Application.Features.Authentications.V1.Models.Requests;
using Readify.Application.Features.Authentications.V1.Models.Response;

namespace Readify.Application.Features.Authentications.V1
{
    public interface IAuthenticationAppServices
    {
        Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
        Task<Result<AuthResponse>> GetTokenByIdAsync(Guid id);
        Task<Result<LogoutResponse>> LogoutAsync();
    }
}
