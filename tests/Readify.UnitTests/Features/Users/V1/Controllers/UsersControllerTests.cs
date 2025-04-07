using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Readify.API.Features.Users.V1.Controllers;
using Readify.Application.Features.Users.V1;
using Readify.Application.Features.Users.V1.Models.Requests;

namespace Readify.UnitTests.Features.Users.V1.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUsersAppServices> _mockUsersAppServices;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockUsersAppServices = new Mock<IUsersAppServices>();
            _controller = new UsersController(_mockUsersAppServices.Object);
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
    }
}
