using Moq;
using Readify.Application.Features.Users.V1.Implementations;
using Readify.Application.Features.Users.V1.Infrastructure.Entities;
using Readify.Application.Features.Users.V1.Infrastructure.IRepositories;
using Readify.Application.Features.Users.V1.Models.Requests;

namespace Readify.UnitTests.Features.Users.V1.ApplicationServices
{
    public class UsersAppServiceTests
    {
        private readonly Mock<IUsersRepository> _mockUsersRepository;
        private readonly UsersAppServices _usersAppServices;

        public UsersAppServiceTests()
        {
            _mockUsersRepository = new Mock<IUsersRepository>();
            _usersAppServices = new UsersAppServices(_mockUsersRepository.Object);
        }

        [Fact]
        public async Task CreateUserAsync_ReturnsFailureResult_WhenRequestIsNull()
        {
            // Act
            var result = await _usersAppServices.CreateUserAsync(null);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("The request cannot be null", result.Errors.First().Message);
        }

        [Theory]
        [InlineData("", "test.user@example.com", "Password123", "2000-01-01")]
        [InlineData("Test User", "", "Password123", "2000-01-01")]
        [InlineData("Test User", "test.user@example.com", "", "2000-01-01")]
        [InlineData("Test User", "test.user@example.com", "Password123", "1800-01-01")]
        [InlineData("Test User", "test.user@example.com", "Password123", "3000-01-01")]
        public async Task CreateUserAsync_ReturnsFailureResult_WhenFieldsAreInvalid(string name, string email, string password, DateTime birthDate)
        {
            // Arrange
            var request = new AddUserRequest
            {
                Name = name,
                Email = email,
                Password = password,
                BirthDate = birthDate
            };

            // Act
            var result = await _usersAppServices.CreateUserAsync(request);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("All fields are required", result.Errors.First().Message);
        }

        [Fact]
        public async Task CreateUserAsync_ReturnsFailureResult_WhenUserAlreadyExists()
        {
            // Arrange
            var request = new AddUserRequest
            {
                Name = "Test User",
                Email = "test.user@example.com",
                Password = "Password123",
                BirthDate = DateTime.UtcNow.AddYears(-30)
            };
            var existingUser = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                Password = request.Password,
                BirthDate = request.BirthDate,
                IsActive = true
            };
            _mockUsersRepository.Setup(repo => repo.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _usersAppServices.CreateUserAsync(request);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("User already exists!", result.Errors.First().Message);
        }

        [Fact]
        public async Task CreateUserAsync_ReturnsSuccessResult_WhenInactiveUserIsReactivated()
        {
            // Arrange
            var request = new AddUserRequest
            {
                Name = "Test User",
                Email = "test.user@example.com",
                Password = "Password123",
                BirthDate = DateTime.UtcNow.AddYears(-30)
            };
            var existingUser = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                Password = "OldPassword",
                BirthDate = request.BirthDate,
                IsActive = false
            };
            _mockUsersRepository.Setup(repo => repo.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(existingUser);
            _mockUsersRepository.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(true);

            // Act
            var result = await _usersAppServices.CreateUserAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(existingUser.Id, result.Value);
        }

        [Fact]
        public async Task CreateUserAsync_ReturnsFailureResult_WhenReactivationFails()
        {
            // Arrange
            var request = new AddUserRequest
            {
                Name = "Test User",
                Email = "test.user@example.com",
                Password = "Password123",
                BirthDate = DateTime.UtcNow.AddYears(-30)
            };
            var existingUser = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                Password = "OldPassword",
                BirthDate = request.BirthDate,
                IsActive = false
            };
            _mockUsersRepository.Setup(repo => repo.GetUserByEmailAsync(request.Email))
                .ReturnsAsync(existingUser);
            _mockUsersRepository.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(false);

            // Act
            var result = await _usersAppServices.CreateUserAsync(request);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("Something went wrong! Try again later.", result.Errors.First().Message);
        }

        [Fact]
        public async Task CreateUserAsync_ReturnsSuccessResult_WhenUserIsCreatedSuccessfully()
        {
            // Arrange
            var request = new AddUserRequest
            {
                Name = "Test User",
                Email = "test.user@example.com",
                Password = "Password123",
                BirthDate = DateTime.UtcNow.AddYears(-30)
            };
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                Password = request.Password,
                BirthDate = request.BirthDate,
                IsActive = true
            };
            _mockUsersRepository.Setup(repo => repo.GetUserByEmailAsync(request.Email))
                .ReturnsAsync((User)null);
            _mockUsersRepository.Setup(repo => repo.AddAsync(It.IsAny<User>()))
                .ReturnsAsync(newUser.Id);

            // Act
            var result = await _usersAppServices.CreateUserAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newUser.Id, result.Value);
        }

        [Fact]
        public async Task CreateUserAsync_ReturnsFailureResult_WhenUserCreationFails()
        {
            // Arrange
            var request = new AddUserRequest
            {
                Name = "Test User",
                Email = "test.user@example.com",
                Password = "Password123",
                BirthDate = DateTime.UtcNow.AddYears(-30)
            };
            _mockUsersRepository.Setup(repo => repo.GetUserByEmailAsync(request.Email))
                .ReturnsAsync((User)null);
            _mockUsersRepository.Setup(repo => repo.AddAsync(It.IsAny<User>()))
                .ReturnsAsync((Guid?)null);

            // Act
            var result = await _usersAppServices.CreateUserAsync(request);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("Something went wrong! Try again later.", result.Errors.First().Message);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsFailureResult_WhenNoUsersFound()
        {
            // Arrange
            _mockUsersRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<User>());

            // Act
            var result = await _usersAppServices.GetAllUsersAsync();

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("No users found!", result.Errors.First().Message);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsFailureResult_WhenRepositoryReturnsNull()
        {
            // Arrange
            _mockUsersRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync((List<User>)null);

            // Act
            var result = await _usersAppServices.GetAllUsersAsync();

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("No users found!", result.Errors.First().Message);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsSuccessResult_WithUsersList()
        {
            // Arrange
            var users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Test User 1",
                    Email = "test.user1@example.com",
                    BirthDate = DateTime.UtcNow.AddYears(-30),
                    Password = "Password123",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Test User 2",
                    Email = "test.user2@example.com",
                    BirthDate = DateTime.UtcNow.AddYears(-25),
                    Password = "Password123",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            };
            _mockUsersRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(users);

            // Act
            var result = await _usersAppServices.GetAllUsersAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count);
            Assert.Equal(users[0].Id, result.Value[0].Id);
            Assert.Equal(users[1].Id, result.Value[1].Id);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsFailureResult_WhenIdIsEmpty()
        {
            // Act
            var result = await _usersAppServices.GetUserByIdAsync(Guid.Empty);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("The id cannot be empty.", result.Errors.First().Message);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsFailureResult_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockUsersRepository.Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _usersAppServices.GetUserByIdAsync(userId);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("User not found!", result.Errors.First().Message);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsSuccessResult_WithUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Name = "Test User",
                Email = "test.user@example.com",
                BirthDate = DateTime.UtcNow.AddYears(-30),
                Password = "Password123",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            _mockUsersRepository.Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _usersAppServices.GetUserByIdAsync(userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(userId, result.Value.Id);
            Assert.Equal(user.Name, result.Value.Name);
            Assert.Equal(user.Email, result.Value.Email);
            Assert.Equal(user.CreatedAt, result.Value.CreatedAt);
            Assert.Equal(user.IsActive, result.Value.IsActive);
        }
    }
}
