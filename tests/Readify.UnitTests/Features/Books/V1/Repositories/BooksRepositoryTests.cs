using Microsoft.EntityFrameworkCore;
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

        [Fact]
        public async Task GetAllAsync_ReturnsListOfBooks_WhenBooksExist()
        {
            // Arrange
            var count = _context.Books.Count();

            var book1 = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Title 1",
                Author = "Test Author 1",
                Genre = "Test Genre 1",
                PublishDate = DateTime.UtcNow,
                Status = true
            };
            
            var book2 = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Title 2",
                Author = "Test Author 2",
                Genre = "Test Genre 2",
                PublishDate = DateTime.UtcNow,
                Status = true
            };
            
            await _context.Books.AddRangeAsync(book1, book2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _booksRepository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(count+2, result.Count);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoBooksExist()
        {
            // Arrange
            await _context.Books.ForEachAsync(b => _context.Remove(b));
            await _context.SaveChangesAsync();

            // Act
            var result = await _booksRepository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetBookByIdAsync_ReturnsBook_WhenBookExists()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book
            {
                Id = bookId,
                Title = "Test Title",
                Author = "Test Author",
                Genre = "Test Genre",
                PublishDate = DateTime.UtcNow,
                Status = true
            };
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();

            // Act
            var result = await _booksRepository.GetBookByIdAsync(bookId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bookId, result.Id);
        }

        [Fact]
        public async Task GetBookByIdAsync_ReturnsNull_WhenBookDoesNotExist()
        {
            // Arrange
            var bookId = Guid.NewGuid();

            // Act
            var result = await _booksRepository.GetBookByIdAsync(bookId);

            // Assert
            Assert.Null(result);
        }
    }
}
