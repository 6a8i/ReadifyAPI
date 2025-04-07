using Microsoft.EntityFrameworkCore;
using Readify.Application.Features.Users.V1.Infrastructure.Entities;
using Readify.Infrastructure.Commons.DatabaseContexts.V1;
using Readify.Infrastructure.Contexts.Users.V1.Repositories;

namespace Readify.UnitTests.Features.Users.V1.Repositories
{
    public class UsersRepositoryTests
    {
        private readonly DbContextOptions<ReadifyDatabaseContext> _dbContextOptions;
        private readonly ReadifyDatabaseContext _context;
        private readonly UsersRepository _usersRepository;

        public UsersRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ReadifyDatabaseContext>()
                .UseInMemoryDatabase(databaseName: "ReadifyTestDb")
                .Options;
            _context = new ReadifyDatabaseContext(_dbContextOptions);
            _usersRepository = new UsersRepository(_context);
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
    }
}
