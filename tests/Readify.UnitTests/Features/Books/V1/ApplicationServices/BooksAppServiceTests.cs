using Moq;
using Readify.Application.Features.Books.V1.Implementations;
using Readify.Application.Features.Books.V1.Infrastructure.Entities;
using Readify.Application.Features.Books.V1.Infrastructure.IRepositories;
using Readify.Application.Features.Books.V1.Models.Requests;

namespace Readify.UnitTests.Features.Books.V1.ApplicationServices
{
    public class BooksAppServicesTests
    {
        private readonly Mock<IBooksRepository> _mockBooksRepository;
        private readonly BooksAppServices _booksAppServices;

        public BooksAppServicesTests()
        {
            _mockBooksRepository = new Mock<IBooksRepository>();
            _booksAppServices = new BooksAppServices(_mockBooksRepository.Object);
        }

        [Fact]
        public async Task CreateABookAsync_ReturnsSuccessResult_WithGuid()
        {
            // Arrange
            var request = new AddBookRequest
            {
                Title = "Test Title",
                Author = "Test Author",
                Genre = "Test Genre",
                PublishDate = DateTime.UtcNow
            };
            var expectedGuid = Guid.NewGuid();
            _mockBooksRepository.Setup(repo => repo.AddAsync(It.IsAny<Book>()))
                .ReturnsAsync(expectedGuid);

            // Act
            var result = await _booksAppServices.CreateABookAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedGuid, result.Value);
        }

        [Theory]
        [InlineData("", "Test Author", "Test Genre", "2023-10-01")]
        [InlineData("Test Title", "", "Test Genre", "2023-10-01")]
        [InlineData("Test Title", "Test Author", "", "2023-10-01")]
        [InlineData("Test Title", "Test Author", "Test Genre", "0001-01-01")]        
        [InlineData(null, null, null, null)]
        public async Task CreateABookAsync_ReturnsFailureResult_WithError(string title, string author, string genre, DateTime publishDate)
        {
            // Arrange
            var request = new AddBookRequest
            {
                Title = title,
                Author = author,
                Genre = genre,
                PublishDate = publishDate
            };

            // Act
            var result = await _booksAppServices.CreateABookAsync(request);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("All fields are required", result.Errors[0].Message);
        }

        [Theory]
        [InlineData("Test Title", "Test Author", "Test Genre", "9999-12-31")]
        public async Task CreateABookAsync_ReturnsFailureResult_WhenDatabaseHasExceptions(string title, string author, string genre, DateTime publishDate)
        {
            // Arrange
            var request = new AddBookRequest
            {
                Title = title,
                Author = author,
                Genre = genre,
                PublishDate = publishDate
            };

            // Act
            var result = await _booksAppServices.CreateABookAsync(request);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("Something went wrong! Try again later.", result.Errors[0].Message);
        }

        [Fact]
        public async Task CreateABookAsync_ReturnsFailureResult_WhenRepositoryReturnsNull()
        {
            // Arrange
            var request = new AddBookRequest
            {
                Title = "Test Title",
                Author = "Test Author",
                Genre = "Test Genre",
                PublishDate = DateTime.UtcNow
            };
            _mockBooksRepository.Setup(repo => repo.AddAsync(It.IsAny<Book>()))
                .ReturnsAsync((Guid?)null);

            // Act
            var result = await _booksAppServices.CreateABookAsync(request);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("Something went wrong! Try again later.", result.Errors[0].Message);
        }

        [Fact]
        public async Task GetAllBooksAsync_ReturnsSuccessResult_WithBooksList()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book
                {
                    Id = Guid.NewGuid(),
                    Title = "Test Title 1",
                    Author = "Test Author 1",
                    Genre = "Test Genre 1",
                    PublishDate = DateTime.UtcNow,
                    Status = true
                },
                new Book
                {
                    Id = Guid.NewGuid(),
                    Title = "Test Title 2",
                    Author = "Test Author 2",
                    Genre = "Test Genre 2",
                    PublishDate = DateTime.UtcNow,
                    Status = true
                }
            };
            _mockBooksRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(books);

            // Act
            var result = await _booksAppServices.GetAllBooksAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count);
        }

        [Fact]
        public async Task GetAllBooksAsync_ReturnsFailureResult_WhenNoBooksFound()
        {
            // Arrange
            _mockBooksRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Book>());

            // Act
            var result = await _booksAppServices.GetAllBooksAsync();

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("No books found!", result.Errors.First().Message);
        }

        [Fact]
        public async Task GetAllBooksAsync_ReturnsFailureResult_WhenRepositoryReturnsNull()
        {
            // Arrange
            _mockBooksRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync((List<Book>)null);

            // Act
            var result = await _booksAppServices.GetAllBooksAsync();

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("No books found!", result.Errors.First().Message);
        }
    }
}
