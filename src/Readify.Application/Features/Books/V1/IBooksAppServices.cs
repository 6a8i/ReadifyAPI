using FluentResults;
using Readify.Application.Features.Books.V1.Models.Requests;

namespace Readify.Application.Features.Books.V1
{
    public interface IBooksAppServices
    {
        Task<Result<Guid>> CreateABookAsync(AddBookRequest request);
    }
}
