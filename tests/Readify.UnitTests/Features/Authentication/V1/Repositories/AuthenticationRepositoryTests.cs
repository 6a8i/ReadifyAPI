using Microsoft.EntityFrameworkCore;
using Readify.Application.Features.Authentications.V1.Infrastructure.Entities;
using Readify.Infrastructure.Commons.DatabaseContexts.V1;
using Readify.Infrastructure.Contexts.Authentication.V1.Repositories;

namespace Readify.UnitTests.Features.Authentication.V1.Repositories
{
    public class AuthenticationRepositoryTests
    {
        private readonly DbContextOptions<ReadifyDatabaseContext> _dbContextOptions;
        private readonly ReadifyDatabaseContext _context;
        private readonly AuthenticationRepository _authenticationRepository;

        public AuthenticationRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ReadifyDatabaseContext>()
                .UseInMemoryDatabase(databaseName: "ReadifyTestDb")
                .Options;
            _context = new ReadifyDatabaseContext(_dbContextOptions);
            _authenticationRepository = new AuthenticationRepository(_context);
        }

        [Fact]
        public async Task CreateTokenAsync_ReturnsTokenId_WhenTokenIsCreatedSuccessfully()
        {
            // Arrange
            var token = new Token
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(8),
                HasExpired = false
            };

            // Act
            var result = await _authenticationRepository.CreateTokenAsync(token);

            // Assert
            Assert.NotEqual(Guid.Empty, result);
            Assert.Equal(token.Id, result);
        }

        [Fact]
        public async Task CreateTokenAsync_AddsTokenToDatabase()
        {
            // Arrange
            var token = new Token
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(8),
                HasExpired = false
            };

            // Act
            await _authenticationRepository.CreateTokenAsync(token);
            var addedToken = await _context.Tokens.FindAsync(token.Id);

            // Assert
            Assert.NotNull(addedToken);
            Assert.Equal(token.Id, addedToken.Id);
            Assert.Equal(token.UserId, addedToken.UserId);
            Assert.Equal(token.CreatedAt, addedToken.CreatedAt);
            Assert.Equal(token.ExpiresAt, addedToken.ExpiresAt);
            Assert.Equal(token.HasExpired, addedToken.HasExpired);
        }
    }
}
