using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Readify.API.Features.Users.V1.Controllers;
using Readify.Application.Features.Authentications.V1;
using Readify.Application.Features.Authentications.V1.Models.Requests;
using Readify.Application.Features.Authentications.V1.Models.Response;
using Readify.Application.Features.Users.V1;
using Readify.Application.Features.Users.V1.Models.Requests;
using Readify.Application.Features.Users.V1.Models.Responses;

namespace Readify.UnitTests.Features.Users.V1.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUsersAppServices> _mockUsersAppServices;
        private readonly Mock<IAuthenticationAppServices> _mockAuthenticationAppServices;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockUsersAppServices = new Mock<IUsersAppServices>();
            _mockAuthenticationAppServices = new Mock<IAuthenticationAppServices>();
            _controller = new UsersController(_mockUsersAppServices.Object, _mockAuthenticationAppServices.Object);
        }

        [Fact]
        public async Task PostAUser_ReturnsOkResult_WithGuid()
        {
            // Arrange
            var request = new AddUserRequest
            {
                Name = "Test",
                Email = "test.user@example.com",
                Password = "Password123"
            };
            var expectedGuid = Guid.NewGuid();
            _mockUsersAppServices.Setup(service => service.CreateUserAsync(request))
                .ReturnsAsync(Result.Ok<Guid?>(expectedGuid));

            // Act
            var result = await _controller.PostAUser(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsType<Result<Guid?>>(okResult.Value);
            Assert.True(resultValue.IsSuccess);
            Assert.Equal(expectedGuid, resultValue.Value);
        }

        [Fact]
        public async Task PostAUser_ReturnsBadRequest_WhenServiceFails()
        {
            // Arrange
            var request = new AddUserRequest
            {
                Name = "Test",
                Email = "test.user@example.com",
                Password = "Password123"
            };
            var expectedError = new Error("Service failed");
            _mockUsersAppServices.Setup(service => service.CreateUserAsync(request))
                .ReturnsAsync(Result.Fail<Guid?>(expectedError));

            // Act
            var result = await _controller.PostAUser(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var resultValue = Assert.IsType<Error>(badRequestResult.Value);
            Assert.Equal(expectedError, resultValue);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOkResult_WithUsersList()
        {
            // Arrange
            var users = new List<GetUserResponse>
            {
                new GetUserResponse
                {
                    Id = Guid.NewGuid(),
                    Name = "Test User 1",
                    Email = "test.user1@example.com",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new GetUserResponse
                {
                    Id = Guid.NewGuid(),
                    Name = "Test User 2",
                    Email = "test.user2@example.com",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            };
            _mockUsersAppServices.Setup(service => service.GetAllUsersAsync())
                .ReturnsAsync(Result.Ok(users));

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsType<Result<List<GetUserResponse>>>(okResult.Value);
            Assert.True(resultValue.IsSuccess);
            Assert.Equal(2, resultValue.Value.Count);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsBadRequest_WhenServiceFails()
        {
            // Arrange
            var expectedError = new Error("Service failed");
            _mockUsersAppServices.Setup(service => service.GetAllUsersAsync())
                .ReturnsAsync(Result.Fail<List<GetUserResponse>>(expectedError));

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var resultValue = Assert.IsType<Error>(badRequestResult.Value);
            Assert.Equal(expectedError, resultValue);
        }

        [Fact]
        public async Task Login_ReturnsOkResult_WithAuthResponse()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test.user@example.com",
                Password = "Password123"
            };

            var authResponse = new AuthResponse
            {
                Token = Guid.NewGuid(),
                TokenCreatedAt = DateTime.UtcNow,
                TokenExpiresAt = DateTime.UtcNow.AddHours(1),
                UserId = Guid.NewGuid(),
                TokenHasExpired = false,
            };

            _mockAuthenticationAppServices.Setup(service => service.LoginAsync(request))
                .ReturnsAsync(Result.Ok(authResponse));

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsType<Result<AuthResponse>>(okResult.Value);
            Assert.True(resultValue.IsSuccess);
            Assert.Equal(authResponse.Token, resultValue.Value.Token);
            Assert.Equal(authResponse.TokenExpiresAt, resultValue.Value.TokenExpiresAt);
            Assert.Equal(authResponse.UserId, resultValue.Value.UserId);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenServiceFails()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test.user@example.com",
                Password = "Password123"
            };
            var expectedError = new Error("Invalid credentials");
            _mockAuthenticationAppServices.Setup(service => service.LoginAsync(request))
                .ReturnsAsync(Result.Fail<AuthResponse>(expectedError));

            // Act
            var result = await _controller.Login(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var resultValue = Assert.IsType<Error>(badRequestResult.Value);
            Assert.Equal(expectedError, resultValue);
        }
    }
}
