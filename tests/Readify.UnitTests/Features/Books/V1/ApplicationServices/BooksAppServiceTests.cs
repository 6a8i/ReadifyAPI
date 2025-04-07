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

        [Fact]
        public async Task GetBookByIdAsync_ReturnsSuccessResult_WithBook()
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
            _mockBooksRepository.Setup(repo => repo.GetBookByIdAsync(bookId))
                .ReturnsAsync(book);

            // Act
            var result = await _booksAppServices.GetBookByIdAsync(bookId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(bookId, result.Value.Id);
        }

        [Fact]
        public async Task GetBookByIdAsync_ReturnsFailureResult_WhenBookNotFound()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            _mockBooksRepository.Setup(repo => repo.GetBookByIdAsync(bookId))
                .ReturnsAsync((Book)null);

            // Act
            var result = await _booksAppServices.GetBookByIdAsync(bookId);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("Book not found!", result.Errors.First().Message);
        }

        [Fact]
        public async Task UpdateBookByIdAsync_ReturnsSuccessResult_WithUpdatedBook()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var request = new UpdateBookRequest
            {
                Title = "Updated Title",
                Author = "Updated Author",
                Genre = "Updated Genre",
                PublishDate = DateTime.UtcNow,
                Status = true
            };
            var existingBook = new Book
            {
                Id = bookId,
                Title = "Original Title",
                Author = "Original Author",
                Genre = "Original Genre",
                PublishDate = DateTime.UtcNow.AddDays(-1),
                Status = false
            };
            var updatedBook = new Book
            {
                Id = bookId,
                Title = request.Title,
                Author = request.Author,
                Genre = request.Genre,
                PublishDate = request.PublishDate.Value,
                Status = request.Status.Value
            };
            _mockBooksRepository.Setup(repo => repo.GetBookByIdAsync(bookId))
                .ReturnsAsync(existingBook);
            _mockBooksRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Book>()))
                .ReturnsAsync(updatedBook);

            // Act
            var result = await _booksAppServices.UpdateBookByIdAsync(bookId, request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(bookId, result.Value.Id);
            Assert.Equal(request.Title, result.Value.Title);
            Assert.Equal(request.Author, result.Value.Author);
            Assert.Equal(request.Genre, result.Value.Genre);
            Assert.Equal(request.PublishDate, result.Value.PublishDate);
            Assert.Equal(request.Status, result.Value.Status);
        }

        [Fact]
        public async Task UpdateBookByIdAsync_ReturnsFailureResult_WhenRequestIsNull()
        {
            // Arrange
            var bookId = Guid.NewGuid();

            // Act
            var result = await _booksAppServices.UpdateBookByIdAsync(bookId, null);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("The request can't be null.", result.Errors.First().Message);
        }

        [Fact]
        public async Task UpdateBookByIdAsync_ReturnsFailureResult_WhenBookNotFound()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var request = new UpdateBookRequest
            {
                Title = "Updated Title",
                Author = "Updated Author",
                Genre = "Updated Genre",
                PublishDate = DateTime.UtcNow,
                Status = true
            };
            _mockBooksRepository.Setup(repo => repo.GetBookByIdAsync(bookId))
                .ReturnsAsync((Book)null);

            // Act
            var result = await _booksAppServices.UpdateBookByIdAsync(bookId, request);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("Book not found!", result.Errors.First().Message);
        }

        [Fact]
        public async Task UpdateBookByIdAsync_ReturnsFailureResult_WhenUpdateFails()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var request = new UpdateBookRequest
            {
                Title = "Updated Title",
                Author = "Updated Author",
                Genre = "Updated Genre",
                PublishDate = DateTime.UtcNow,
                Status = true
            };
            var existingBook = new Book
            {
                Id = bookId,
                Title = "Original Title",
                Author = "Original Author",
                Genre = "Original Genre",
                PublishDate = DateTime.UtcNow.AddDays(-1),
                Status = false
            };
            _mockBooksRepository.Setup(repo => repo.GetBookByIdAsync(bookId))
                .ReturnsAsync(existingBook);
            _mockBooksRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Book>()))
                .ReturnsAsync((Book)null);

            // Act
            var result = await _booksAppServices.UpdateBookByIdAsync(bookId, request);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("Something went wrong! Try again later.", result.Errors.First().Message);
        }

        [Fact]
        public async Task DeleteBookByIdAsync_ReturnsFailureResult_WhenIdIsEmpty()
        {
            // Arrange
            var emptyId = Guid.Empty;

            // Act
            var result = await _booksAppServices.DeleteBookByIdAsync(emptyId);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("The id cannot be empty.", result.Errors.First().Message);
        }

        [Fact]
        public async Task DeleteBookByIdAsync_ReturnsFailureResult_WhenBookNotFound()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            _mockBooksRepository.Setup(repo => repo.GetBookByIdAsync(bookId))
                .ReturnsAsync((Book)null);

            // Act
            var result = await _booksAppServices.DeleteBookByIdAsync(bookId);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("Book not found!", result.Errors.First().Message);
        }

        [Fact]
        public async Task DeleteBookByIdAsync_ReturnsFailureResult_WhenBookCannotBeDeleted()
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
                Status = false // Book is borrowed and not returned
            };
            _mockBooksRepository.Setup(repo => repo.GetBookByIdAsync(bookId))
                .ReturnsAsync(book);

            // Act
            var result = await _booksAppServices.DeleteBookByIdAsync(bookId);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("Book cannot be deleted as it is currently borrowed.", result.Errors.First().Message);
        }

        [Fact]
        public async Task DeleteBookByIdAsync_ReturnsFailureResult_WhenDeletionFails()
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
                Status = true // Book can be deleted
            };
            _mockBooksRepository.Setup(repo => repo.GetBookByIdAsync(bookId))
                .ReturnsAsync(book);
            _mockBooksRepository.Setup(repo => repo.DeleteAsync(book))
                .ReturnsAsync(false);

            // Act
            var result = await _booksAppServices.DeleteBookByIdAsync(bookId);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal("Something went wrong! Try again later.", result.Errors.First().Message);
        }

        [Fact]
        public async Task DeleteBookByIdAsync_ReturnsSuccessResult_WhenDeletionIsSuccessful()
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
                Status = true // Book can be deleted
            };
            _mockBooksRepository.Setup(repo => repo.GetBookByIdAsync(bookId))
                .ReturnsAsync(book);
            _mockBooksRepository.Setup(repo => repo.DeleteAsync(book))
                .ReturnsAsync(true);

            // Act
            var result = await _booksAppServices.DeleteBookByIdAsync(bookId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal($"The book {book.Title} of id: {book.Id} was successfully deleted!", result.Successes.First().Message);
        }
    }
}
