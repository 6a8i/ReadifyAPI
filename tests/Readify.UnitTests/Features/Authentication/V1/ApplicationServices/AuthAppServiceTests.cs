using FluentResults;
using Moq;
using Readify.Application.Features.Authentications.V1.Implementations;
using Readify.Application.Features.Authentications.V1.Infrastructure.Entities;
using Readify.Application.Features.Authentications.V1.Infrastructure.IRepositories;
using Readify.Application.Features.Authentications.V1.Models.Requests;
using Readify.Application.Features.Authentications.V1.Models.Response;
using Readify.Application.Features.Authentications.V1.Models.Statics;
using Readify.Application.Features.Users.V1;
using Readify.Application.Features.Users.V1.Models.Responses;

namespace Readify.UnitTests.Features.Authentication.V1.ApplicationServices
{
    public class AuthAppServiceTests
    {
        private readonly Mock<IAuthenticationRepository> _mockAuthenticationRepository;
        private readonly Mock<IUsersAppServices> _mockUsersAppServices;
        private readonly AuthenticationAppServices _authAppServices;

        public AuthAppServiceTests()
        {
            _mockAuthenticationRepository = new Mock<IAuthenticationRepository>();
            _mockUsersAppServices = new Mock<IUsersAppServices>();
            _authAppServices = new AuthenticationAppServices(_mockAuthenticationRepository.Object, _mockUsersAppServices.Object);
        }

        [Fact]
        public async Task GetTokenByIdAsync_ReturnsFailureResult_WhenIdIsEmpty()
        {
            // Act
            var result = await _authAppServices.GetTokenByIdAsync(Guid.Empty);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("Authorization cannot be empty.", result.Errors.First().Message);
        }

        [Fact]
        public async Task GetTokenByIdAsync_ReturnsFailureResult_WhenTokenNotFound()
        {
            // Arrange
            var tokenId = Guid.NewGuid();
            _mockAuthenticationRepository.Setup(repo => repo.GetTokenByIdAsync(tokenId))
                .ReturnsAsync((Token?)null);

            // Act
            var result = await _authAppServices.GetTokenByIdAsync(tokenId);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("Unauthorized.", result.Errors.First().Message);
        }

        [Fact]
        public async Task GetTokenByIdAsync_ReturnsFailureResult_WhenTokenHasExpired()
        {
            // Arrange
            var tokenId = Guid.NewGuid();
            var expiredToken = new Token
            {
                Id = tokenId,
                ExpiresAt = DateTime.UtcNow.AddHours(-1),
                HasExpired = false
            };
            _mockAuthenticationRepository.Setup(repo => repo.GetTokenByIdAsync(tokenId))
                .ReturnsAsync(expiredToken);
            _mockAuthenticationRepository.Setup(repo => repo.UpdateTokenAsync(It.IsAny<Token>()))
                .Returns(Task.FromResult(true));

            // Act
            var result = await _authAppServices.GetTokenByIdAsync(tokenId);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("Token has expired.", result.Errors.First().Message);
            Assert.True(expiredToken.HasExpired);
        }

        [Fact]
        public async Task GetTokenByIdAsync_ReturnsSuccessResult_WithAuthResponse()
        {
            // Arrange
            var tokenId = Guid.NewGuid();
            var validToken = new Token
            {
                Id = tokenId,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                HasExpired = false
            };
            _mockAuthenticationRepository.Setup(repo => repo.GetTokenByIdAsync(tokenId))
                .ReturnsAsync(validToken);

            // Act
            var result = await _authAppServices.GetTokenByIdAsync(tokenId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(validToken.Id, result.Value.Token);
            Assert.Equal(validToken.ExpiresAt, result.Value.TokenExpiresAt);
        }

        [Fact]
        public async Task LoginAsync_ReturnsFailureResult_WhenRequestIsNull()
        {
            // Act
            var result = await _authAppServices.LoginAsync(null);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("Request cannot be null.", result.Errors.First().Message);
        }

        [Fact]
        public async Task LoginAsync_ReturnsFailureResult_WhenEmailOrPasswordIsEmpty()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "",
                Password = ""
            };

            // Act
            var result = await _authAppServices.LoginAsync(request);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("Email and Password cannot be empty.", result.Errors.First().Message);
        }

        [Fact]
        public async Task LoginAsync_ReturnsFailureResult_WhenUserNotFound()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "nonexistent.user@example.com",
                Password = "Password123"
            };
            _mockUsersAppServices.Setup(service => service.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(Result.Fail<GetUserResponse>("User not found!"));

            // Act
            var result = await _authAppServices.LoginAsync(request);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("User not found!", result.Errors.First().Message);
        }

        [Fact]
        public async Task LoginAsync_ReturnsFailureResult_WhenPasswordIsIncorrect()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test.user@example.com",
                Password = "WrongPassword"
            };
            var userResponse = new GetUserResponse
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = request.Email,
                IsActive = true
            };
            _mockUsersAppServices.Setup(service => service.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(Result.Ok(userResponse));
            _mockUsersAppServices.Setup(service => service.GetUserPasswordAsync(userResponse.Id))
                .ReturnsAsync(Result.Ok("CorrectPassword"));

            // Act
            var result = await _authAppServices.LoginAsync(request);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("Email or passord is incorrect.", result.Errors.First().Message);
        }

        [Fact]
        public async Task LoginAsync_ReturnsSuccessResult_WithExistingToken()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test.user@example.com",
                Password = "Password123"
            };
            var userResponse = new GetUserResponse
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = request.Email,
                IsActive = true
            };
            var existingToken = new Token
            {
                Id = Guid.NewGuid(),
                UserId = userResponse.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                HasExpired = false
            };
            _mockUsersAppServices.Setup(service => service.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(Result.Ok(userResponse));
            _mockUsersAppServices.Setup(service => service.GetUserPasswordAsync(userResponse.Id))
                .ReturnsAsync(Result.Ok(request.Password));
            _mockAuthenticationRepository.Setup(repo => repo.GetTokenByUserIdAsync(userResponse.Id))
                .ReturnsAsync(existingToken);

            // Act
            var result = await _authAppServices.LoginAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(existingToken.Id, result.Value.Token);
            Assert.Equal(existingToken.ExpiresAt, result.Value.TokenExpiresAt);
        }

        [Fact]
        public async Task LoginAsync_ReturnsSuccessResult_WithNewToken()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test.user@example.com",
                Password = "Password123"
            };
            var userResponse = new GetUserResponse
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = request.Email,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            var newToken = new Token
            {
                Id = Guid.NewGuid(),
                UserId = userResponse.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(8),
                HasExpired = false
            };
            _mockUsersAppServices.Setup(service => service.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(Result.Ok(userResponse));
            _mockUsersAppServices.Setup(service => service.GetUserPasswordAsync(userResponse.Id))
                .ReturnsAsync(Result.Ok(request.Password));
            _mockAuthenticationRepository.Setup(repo => repo.GetTokenByUserIdAsync(userResponse.Id))
                .ReturnsAsync((Token?)null);
            _mockAuthenticationRepository.Setup(repo => repo.CreateTokenAsync(It.IsAny<Token>()))
                .ReturnsAsync(newToken.Id);

            // Act
            var result = await _authAppServices.LoginAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newToken.Id, result.Value.Token);
            Assert.Equal(result.Value.TokenCreatedAt.AddHours(8), result.Value.TokenExpiresAt);
        }

        [Fact]
        public async Task LoginAsync_ReturnsFailureResult_WhenTokenCreationFails()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test.user@example.com",
                Password = "Password123"
            };
            var userResponse = new GetUserResponse
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = request.Email,
                IsActive = true
            };
            _mockUsersAppServices.Setup(service => service.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(Result.Ok(userResponse));
            _mockUsersAppServices.Setup(service => service.GetUserPasswordAsync(userResponse.Id))
                .ReturnsAsync(Result.Ok(request.Password));
            _mockAuthenticationRepository.Setup(repo => repo.GetTokenByUserIdAsync(userResponse.Id))
                .ReturnsAsync((Token?)null);
            _mockAuthenticationRepository.Setup(repo => repo.CreateTokenAsync(It.IsAny<Token>()))
                .ReturnsAsync(Guid.Empty);

            // Act
            var result = await _authAppServices.LoginAsync(request);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("Something went wrong, try again later.", result.Errors.First().Message);
        }

        [Fact]
        public async Task LogoutAsync_ReturnsFailureResult_WhenAuthContextIsNull()
        {
            // Arrange
            AuthManager.Context = null;

            // Act
            var result = await _authAppServices.LogoutAsync();

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("No login or authentication was made.", result.Errors.First().Message);
        }

        [Fact]
        public async Task LogoutAsync_ReturnsFailureResult_WhenUserNotFound()
        {
            // Arrange
            AuthManager.Context = new AuthResponse { UserId = Guid.NewGuid() };
            _mockUsersAppServices.Setup(service => service.GetUserByIdAsync(AuthManager.Context.UserId))
                .ReturnsAsync(Result.Fail<GetUserResponse>("User not found!"));

            // Act
            var result = await _authAppServices.LogoutAsync();

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("User not found!", result.Errors.First().Message);
        }

        [Fact]
        public async Task LogoutAsync_ReturnsFailureResult_WhenTokenExpirationFails()
        {
            // Arrange
            AuthManager.Context = new AuthResponse { UserId = Guid.NewGuid() };
            var userResponse = new GetUserResponse
            {
                Id = AuthManager.Context.UserId,
                Name = "Test User",
                Email = "test.user@example.com",
                IsActive = true
            };
            _mockUsersAppServices.Setup(service => service.GetUserByIdAsync(AuthManager.Context.UserId))
                .ReturnsAsync(Result.Ok(userResponse));
            _mockAuthenticationRepository.Setup(repo => repo.ExpiresAllTokensByUserAsync(AuthManager.Context.UserId))
                .ReturnsAsync(false);

            // Act
            var result = await _authAppServices.LogoutAsync();

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("Something went wrong, try again later.", result.Errors.First().Message);
        }

        [Fact]
        public async Task LogoutAsync_ReturnsSuccessResult_WhenLogoutIsSuccessful()
        {
            // Arrange
            AuthManager.Context = new AuthResponse
            {
                UserId = Guid.NewGuid(),
                Token = Guid.NewGuid(),
                TokenHasExpired = false,
                TokenExpiresAt = DateTime.UtcNow.AddHours(1)
            };
            var userResponse = new GetUserResponse
            {
                Id = AuthManager.Context.UserId,
                Name = "Test User",
                Email = "test.user@example.com",
                IsActive = true
            };
            _mockUsersAppServices.Setup(service => service.GetUserByIdAsync(AuthManager.Context.UserId))
                .ReturnsAsync(Result.Ok(userResponse));
            _mockAuthenticationRepository.Setup(repo => repo.ExpiresAllTokensByUserAsync(AuthManager.Context.UserId))
                .ReturnsAsync(true);

            // Act
            var result = await _authAppServices.LogoutAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(AuthManager.Context.Token, result.Value.Token);
            Assert.True(result.Value.TokenHasExpired);
        }
    }
}
