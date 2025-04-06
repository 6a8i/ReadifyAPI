using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using Readify.Application.Features.Books.V1.Infrastructure.Entities;
using Readify.Infrastructure.Commons.DatabaseContexts.V1;
using Readify.Infrastructure.Contexts.Books.V1.Repositories;

namespace Readify.UnitTests.Features.Books.V1.Repositories
{
    public class BooksRepositoryTests
    {
        private readonly DbContextOptions<ReadifyDatabaseContext> _dbContextOptions;
        private readonly ReadifyDatabaseContext _context;
        private readonly BooksRepository _booksRepository;

        public BooksRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ReadifyDatabaseContext>()
                .UseInMemoryDatabase(databaseName: "ReadifyTestDb")
                .Options;
            _context = new ReadifyDatabaseContext(_dbContextOptions);
            _booksRepository = new BooksRepository(_context);
        }

        [Fact]
        public async Task AddAsync_ReturnsGuid_WhenBookIsAddedSuccessfully()
        {
            // Arrange
            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Title",
                Author = "Test Author",
                Genre = "Test Genre",
                PublishDate = DateTime.UtcNow,
                Status = true
            };

            // Act
            var result = await _booksRepository.AddAsync(book);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(book.Id, result);
        }
    }
}
