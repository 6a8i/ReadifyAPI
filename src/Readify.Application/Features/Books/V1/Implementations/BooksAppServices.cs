using FluentResults;
using Readify.Application.Features.Authentications.V1.Models.Statics;
using Readify.Application.Features.Books.V1.Infrastructure.Entities;
using Readify.Application.Features.Books.V1.Infrastructure.IRepositories;
using Readify.Application.Features.Books.V1.Models.Requests;
using ZiggyCreatures.Caching.Fusion;

namespace Readify.Application.Features.Books.V1.Implementations
{
    public class BooksAppServices(IBooksRepository booksRepository, IFusionCache fusionCache) : IBooksAppServices
    {
        private readonly IBooksRepository _booksRepository = booksRepository;
        private readonly IFusionCache _fusionCache = fusionCache; 

        public async Task<Result<Guid>> CreateABookAsync(AddBookRequest request)
        {
            if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Author) || string.IsNullOrEmpty(request.Genre) || request.PublishDate == DateTime.MinValue || request.PublishDate == DateTime.MaxValue)
                return Result.Fail("All fields are required");

            var entity = (Book)request;

            Guid? id = await _booksRepository.AddAsync(entity);

            if (id is null)
                return Result.Fail("Something went wrong! Try again later.");

            return id.Value.ToResult();
        }

        public async Task<Result> DeleteBookByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                return Result.Fail("The id cannot be empty.");

            var entity = await _booksRepository.GetBookByIdAsync(id);

            if(entity is null)
                return Result.Fail("Book not found!");

            if (!entity.Status)
                return Result.Fail("Book cannot be deleted as it is currently borrowed.");

            bool result = await _booksRepository.DeleteAsync(entity);

            if(!result)
                return Result.Fail("Something went wrong! Try again later.");

            return Result.Ok().WithSuccess($"The book {entity.Title} of id: {entity.Id} was successfully deleted!");
        }

        public async Task<Result<List<Models.Responses.Book>>> GetAllBooksAsync()
        {
            string cacheKey = $"{AuthManager.Context!.UserId}-books-all";
            // Check if the data is already cached
            List<Models.Responses.Book>? result = await _fusionCache.GetOrDefaultAsync<List<Models.Responses.Book>?>(cacheKey, (List<Models.Responses.Book>?)null, null, default);

            if(result is not null)
                return Result.Ok(result);

            List<Book> books = await _booksRepository.GetAllAsync();

            if (books is null || books.Count == 0)
                return Result.Fail("No books found!");

            result = [.. books.Select(b => (Models.Responses.Book)b)];

            await _fusionCache.SetAsync(cacheKey, result, new FusionCacheEntryOptions { Duration = TimeSpan.FromDays(1) });

            return Result.Ok(result);
        }

        public async Task<Result<Models.Responses.Book>> GetBookByIdAsync(Guid id)
        {
            Book? book = await _booksRepository.GetBookByIdAsync(id);

            if (book is null)
                return Result.Fail("Book not found!");

            return ((Models.Responses.Book)book).ToResult();
        }

        public async Task<Result<Models.Responses.Book>> UpdateBookByIdAsync(Guid id, UpdateBookRequest request)
        {
            if (request is null)
                return Result.Fail("The request can't be null.");

            var entity = await _booksRepository.GetBookByIdAsync(id);

            if(entity is null)
                return Result.Fail("Book not found!");

            if (!string.IsNullOrEmpty(request.Title))
                entity.Title = request.Title;

            if (!string.IsNullOrEmpty(request.Author))
                entity.Author = request.Author;

            if (!string.IsNullOrEmpty(request.Genre))
                entity.Genre = request.Genre;

            if (request.PublishDate is not null && request.PublishDate > DateTime.MinValue && request.PublishDate < DateTime.MaxValue)
                entity.PublishDate = request.PublishDate!.Value;

            if(request.Status is not null)
                entity.Status = request.Status!.Value;  

            Book? result = await _booksRepository.UpdateAsync(entity);

            if (result is null)
                return Result.Fail("Something went wrong! Try again later.");

            return ((Models.Responses.Book)result).ToResult();
        }
    }
}
