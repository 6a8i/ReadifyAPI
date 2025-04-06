using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Readify.API.Features.Books.V1.Controllers;
using Readify.Application.Features.Books.V1;
using Readify.Application.Features.Books.V1.Models.Requests;
using Readify.Application.Features.Books.V1.Models.Responses;

namespace Readify.UnitTests.Features.Books.V1.Controllers
{
    public class BooksControllerTests
    {
        private readonly Mock<IBooksAppServices> _mockAppServices;
        private readonly BooksController _controller;

        public BooksControllerTests()
        {
            _mockAppServices = new Mock<IBooksAppServices>();
            _controller = new BooksController(_mockAppServices.Object);
        }

        [Fact]
        public async Task PostABook_ReturnsOkResult_WithGuid()
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
            _mockAppServices.Setup(service => service.CreateABookAsync(request))
                .ReturnsAsync(Result.Ok(expectedGuid));

            // Act
            var result = await _controller.PostABook(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsType<Result<Guid>>(okResult.Value);
            Assert.True(resultValue.IsSuccess);
            Assert.Equal(expectedGuid, resultValue.Value);
        }

        [Theory]
        [InlineData("", "Test Author", "Test Genre", "2023-10-01")]
        [InlineData("Test Title", "", "Test Genre", "2023-10-01")]
        [InlineData("Test Title", "Test Author", "", "2023-10-01")]
        [InlineData("Test Title", "Test Author", "Test Genre", null)]
        [InlineData(null, null, null, null)]
        public async Task PostABook_ReturnsBadResult_WithoutGuid(string title, string author, string genre, DateTime publishDate)
        {
            // Arrange
            var request = new AddBookRequest
            {
                Title = title,
                Author = author,
                Genre = genre,
                PublishDate = publishDate
            };

            Error expectedError = new Error("All fields are required");
            _mockAppServices.Setup(service => service.CreateABookAsync(request))
                .ReturnsAsync(expectedError);

            // Act
            var result = await _controller.PostABook(request);

            // Assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            var resultValue = Assert.IsType<Error>(okResult.Value);
            Assert.Equal(expectedError, resultValue);
        }

        [Fact]
        public async Task GetAllBooks_ReturnsOkResult_WithBooksList()
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
            _mockAppServices.Setup(service => service.GetAllBooksAsync())
                .ReturnsAsync(Result.Ok(books));

            // Act
            var result = await _controller.GetAllBooks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsType<Result<List<Book>>>(okResult.Value);
            Assert.True(resultValue.IsSuccess);
            Assert.Equal(2, resultValue.Value.Count);
        }

        [Fact]
        public async Task GetAllBooks_ReturnsBadRequest_WhenServiceFails()
        {
            // Arrange
            var expectedError = new Error("Service failed");
            _mockAppServices.Setup(service => service.GetAllBooksAsync())
                .ReturnsAsync(Result.Fail<List<Book>>(expectedError));

            // Act
            var result = await _controller.GetAllBooks();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var resultValue = Assert.IsType<Error>(badRequestResult.Value);
            Assert.Equal(expectedError, resultValue);
        }

        [Fact]
        public async Task GetBookById_ReturnsOkResult_WithBook()
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
            _mockAppServices.Setup(service => service.GetBookByIdAsync(bookId))
                .ReturnsAsync(Result.Ok(book));

            // Act
            var result = await _controller.GetBookById(bookId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsType<Result<Book>>(okResult.Value);
            Assert.True(resultValue.IsSuccess);
            Assert.Equal(bookId, resultValue.Value.Id);
        }

        [Fact]
        public async Task GetBookById_ReturnsBadRequest_WhenServiceFails()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var expectedError = new Error("Service failed");
            _mockAppServices.Setup(service => service.GetBookByIdAsync(bookId))
                .ReturnsAsync(Result.Fail<Book>(expectedError));

            // Act
            var result = await _controller.GetBookById(bookId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var resultValue = Assert.IsType<Error>(badRequestResult.Value);
            Assert.Equal(expectedError, resultValue);
        }
    }
}
