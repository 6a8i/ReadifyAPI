using FluentResults;
using Readify.Application.Features.Books.V1.Infrastructure.Entities;
using Readify.Application.Features.Books.V1.Infrastructure.IRepositories;
using Readify.Application.Features.Books.V1.Models.Requests;

namespace Readify.Application.Features.Books.V1.Implementations
{
    public class BooksAppServices(IBooksRepository booksRepository) : IBooksAppServices
    {
        private readonly IBooksRepository _booksRepository = booksRepository;


        public async Task<Result<Guid>> CreateABookAsync(AddBookRequest request)
        {
            if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Author) || string.IsNullOrEmpty(request.Genre) || request.PublishDate == DateTime.MinValue || request.PublishDate == DateTime.MaxValue)
                return new Result().WithError("All fields are required");

            var entity = (Book)request;

            Guid? id = await _booksRepository.AddAsync(entity);

            if (id is null)
                return new Result().WithError("Something went wrong! Try again later.");

            return id.Value.ToResult();
        }
    }
}
