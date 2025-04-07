using Microsoft.EntityFrameworkCore;
using Readify.Application.Features.Users.V1.Infrastructure.Entities;
using Readify.Infrastructure.Commons.DatabaseContexts.V1;
using Readify.Infrastructure.Contexts.Users.V1.Repositories;

namespace Readify.UnitTests.Features.Users.V1.Repositories
{
    public class UsersRepositoryTests
    {
        private readonly DbContextOptions<ReadifyDatabaseContext> _dbContextOptions;
        private readonly DbContextOptions<ReadifyDatabaseContext> _emptydbContextOptions;
        private readonly ReadifyDatabaseContext _context;
        private readonly ReadifyDatabaseContext _emptyContext;
        private readonly UsersRepository _usersRepository;
        private readonly UsersRepository _usersRepositoryEmpty;

        public UsersRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ReadifyDatabaseContext>()
                .UseInMemoryDatabase(databaseName: "ReadifyTestDb")
                .Options;
            _emptydbContextOptions = new DbContextOptionsBuilder<ReadifyDatabaseContext>()
                .UseInMemoryDatabase(databaseName: "ReadifyEmptyTestDb")
                .Options;
            _context = new ReadifyDatabaseContext(_dbContextOptions);
            _emptyContext = new ReadifyDatabaseContext(_emptydbContextOptions);
            _usersRepository = new UsersRepository(_context);
            _usersRepositoryEmpty = new UsersRepository(_emptyContext);
        }

        [Fact]
        public async Task AddAsync_ReturnsGuid_WhenUserIsAddedSuccessfully()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test.user@example.com",
                Password = "Password123",
                BirthDate = DateTime.UtcNow.AddYears(-30),
                IsActive = true
            };

            // Act
            var result = await _usersRepository.AddAsync(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result);
        }

        [Fact]
        public async Task AddAsync_AddsUserToDatabase()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test.user@example.com",
                Password = "Password123",
                BirthDate = DateTime.UtcNow.AddYears(-30),
                IsActive = true
            };

            // Act
            await _usersRepository.AddAsync(user);
            var addedUser = await _context.Users.FindAsync(user.Id);

            // Assert
            Assert.NotNull(addedUser);
            Assert.Equal(user.Id, addedUser.Id);
            Assert.Equal(user.Name, addedUser.Name);
            Assert.Equal(user.Email, addedUser.Email);
            Assert.Equal(user.Password, addedUser.Password);
            Assert.Equal(user.BirthDate, addedUser.BirthDate);
            Assert.Equal(user.IsActive, addedUser.IsActive);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsListOfUsers_WhenUsersExist()
        {
            // Arrange
            var user1 = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User 1",
                Email = "test.user1@example.com",
                Password = "Password123",
                BirthDate = DateTime.UtcNow.AddYears(-30),
                IsActive = true
            };

            var user2 = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User 2",
                Email = "test.user2@example.com",
                Password = "Password123",
                BirthDate = DateTime.UtcNow.AddYears(-25),
                IsActive = true
            };

            await _context.Users.AddRangeAsync(user1, user2);
            await _context.SaveChangesAsync();
            
            var userCountAfterInsert = await _context.Users.CountAsync();  

            // Act
            var result = await _usersRepository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userCountAfterInsert, result.Count);
            Assert.Contains(result, u => u.Id == user1.Id);
            Assert.Contains(result, u => u.Id == user2.Id);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoUsersExist()
        {
            // Act
            var result = await _usersRepositoryEmpty.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Name = "Test User",
                Email = "test.user@example.com",
                Password = "Password123",
                BirthDate = DateTime.UtcNow.AddYears(-30),
                IsActive = true
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _usersRepository.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal(user.Name, result.Name);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.Password, result.Password);
            Assert.Equal(user.BirthDate, result.BirthDate);
            Assert.Equal(user.IsActive, result.IsActive);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var result = await _usersRepositoryEmpty.GetUserByIdAsync(userId);

            // Assert
            Assert.Null(result);
        }
    }
}
