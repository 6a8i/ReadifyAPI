using FluentResults;
using Readify.Application.Features.Authentications.V1.Infrastructure.Entities;
using Readify.Application.Features.Authentications.V1.Infrastructure.IRepositories;
using Readify.Application.Features.Authentications.V1.Models.Requests;
using Readify.Application.Features.Authentications.V1.Models.Response;
using Readify.Application.Features.Authentications.V1.Models.Statics;
using Readify.Application.Features.Users.V1;
using Readify.Application.Features.Users.V1.Infrastructure.Entities;
using Readify.Application.Features.Users.V1.Models.Responses;

namespace Readify.Application.Features.Authentications.V1.Implementations
{
    public class AuthenticationAppServices(IAuthenticationRepository authenticationRepository,
                                           IUsersAppServices usersAppServices) : IAuthenticationAppServices
    {
        private readonly IAuthenticationRepository _authenticationRepository = authenticationRepository;
        private readonly IUsersAppServices _usersAppServices = usersAppServices;

        public async Task<Result<AuthResponse>> GetTokenByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return Result.Fail("Authorization cannot be empty.");
            }

            Token? token = await _authenticationRepository.GetTokenByIdAsync(id);

            if (token is null)
            {
                return Result.Fail("Unauthorized.");
            }

            if (token.ExpiresAt < DateTime.UtcNow)
            {
                token.HasExpired = true;
                await _authenticationRepository.UpdateTokenAsync(token);
            }

            if (token.HasExpired)
            {
                return Result.Fail("Token has expired.");
            }

            return Result.Ok((AuthResponse)token);

        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
        {            
            if (request == null)
            {
                return Result.Fail("Request cannot be null.");
            }

            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return Result.Fail("Email and Password cannot be empty.");
            }

            Result<GetUserResponse> user = await _usersAppServices.GetUserByEmailAsync(request.Email);

            if (user.IsFailed)
            {
                return Result.Fail(user.Errors);
            }

            Result<string> userPassword = await _usersAppServices.GetUserPasswordAsync(user.Value.Id);

            if (userPassword.IsFailed)
            {
                return Result.Fail(userPassword.Errors);
            }

            if (userPassword.Value != request.Password)
            {
                return Result.Fail("Email or passord is incorrect.");
            }

            Token? token = await _authenticationRepository.GetTokenByUserIdAsync(user.Value.Id);

            if (token is not null && token.ExpiresAt < DateTime.UtcNow)
            {
                token.HasExpired = true;
                await _authenticationRepository.UpdateTokenAsync(token);
            }
            else if (token is not null)
            {
                return Result.Ok((AuthResponse)token);
            }

            token = new()
            {
                UserId = user.Value.Id,
                CreatedAt = DateTime.UtcNow,
                HasExpired = false
            };

            // Set the expiration time to 8 hours from creation time
            token.ExpiresAt = token.CreatedAt.AddHours(8);

            Guid tokenGuid = await _authenticationRepository.CreateTokenAsync(token);

            if (tokenGuid == Guid.Empty)
            {
                return Result.Fail("Something went wrong, try again later.");
            }

            token.Id = tokenGuid;

            return Result.Ok((AuthResponse)token);
        }

        public async Task<Result<LogoutResponse>> LogoutAsync()
        {
            if (AuthManager.Context is null)
            {
                return Result.Fail("No login or authentication was made.");
            }

            Result<GetUserResponse> user = await _usersAppServices.GetUserByIdAsync(AuthManager.Context.UserId);

            if (user.IsFailed)
            {
                return Result.Fail(user.Errors);
            }

            bool result = await _authenticationRepository.ExpiresAllTokensByUserAsync(AuthManager.Context.UserId);

            if (!result)
            {
                return Result.Fail("Something went wrong, try again later.");
            }

            AuthManager.Context.TokenHasExpired = true;
            AuthManager.Context.TokenExpiresAt = DateTime.UtcNow.AddHours(-1);

            return Result.Ok(new LogoutResponse { Token = AuthManager.Context.Token, TokenHasExpired = true });
        }
    }
}
