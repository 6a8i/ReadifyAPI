﻿using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Readify.API.Features.Users.V1.Controllers;
using Readify.Application.Features.Users.V1;
using Readify.Application.Features.Users.V1.Models.Requests;
using Readify.Application.Features.Users.V1.Models.Responses;

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
        public async Task GetUserById_ReturnsOkResult_WithUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new GetUserResponse
            {
                Id = userId,
                Name = "Test User",
                Email = "test.user@example.com",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            _mockUsersAppServices.Setup(service => service.GetUserByIdAsync(userId))
                .ReturnsAsync(Result.Ok(user));

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsType<Result<GetUserResponse>>(okResult.Value);
            Assert.True(resultValue.IsSuccess);
            Assert.Equal(userId, resultValue.Value.Id);
        }

        [Fact]
        public async Task GetUserById_ReturnsBadRequest_WhenServiceFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedError = new Error("Service failed");
            _mockUsersAppServices.Setup(service => service.GetUserByIdAsync(userId))
                .ReturnsAsync(Result.Fail<GetUserResponse>(expectedError));

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var resultValue = Assert.IsType<Error>(badRequestResult.Value);
            Assert.Equal(expectedError, resultValue);
        }

        [Fact]
        public async Task UpdateUser_ReturnsOkResult_WithUpdatedUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new UpdateUserRequest
            {
                Name = "Updated User",
                Email = "updated.user@example.com",
                Password = "UpdatedPassword123",
                IsActive = true,
                BirthDate = DateTime.UtcNow.AddYears(-25)
            };
            var updatedUser = new GetUserResponse
            {
                Id = userId,
                Name = request.Name,
                Email = request.Email,
                CreatedAt = DateTime.UtcNow,
                IsActive = request.IsActive.Value
            };
            _mockUsersAppServices.Setup(service => service.UpdateUserByIdAsync(userId, request))
                .ReturnsAsync(Result.Ok(updatedUser));

            // Act
            var result = await _controller.UpdateUser(userId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsType<Result<GetUserResponse>>(okResult.Value);
            Assert.True(resultValue.IsSuccess);
            Assert.Equal(userId, resultValue.Value.Id);
            Assert.Equal(request.Name, resultValue.Value.Name);
            Assert.Equal(request.Email, resultValue.Value.Email);
            Assert.Equal(request.IsActive, resultValue.Value.IsActive);
        }

        [Fact]
        public async Task UpdateUser_ReturnsBadRequest_WhenServiceFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new UpdateUserRequest
            {
                Name = "Updated User",
                Email = "updated.user@example.com",
                Password = "UpdatedPassword123",
                IsActive = true,
                BirthDate = DateTime.UtcNow.AddYears(-25)
            };
            var expectedError = new Error("Service failed");
            _mockUsersAppServices.Setup(service => service.UpdateUserByIdAsync(userId, request))
                .ReturnsAsync(Result.Fail<GetUserResponse>(expectedError));

            // Act
            var result = await _controller.UpdateUser(userId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var resultValue = Assert.IsType<List<IError>>(badRequestResult.Value);
            Assert.Contains(expectedError, resultValue);
        }
    }
}
