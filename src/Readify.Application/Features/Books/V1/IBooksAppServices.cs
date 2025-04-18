﻿using FluentResults;
using Readify.Application.Features.Books.V1.Models.Requests;
using Readify.Application.Features.Books.V1.Models.Responses;

namespace Readify.Application.Features.Books.V1
{
    public interface IBooksAppServices
    {
        Task<Result<Guid>> CreateABookAsync(AddBookRequest request);
        Task<Result> DeleteBookByIdAsync(Guid id);
        Task<Result<List<Book>>> GetAllBooksAsync();
        Task<Result<Book>> GetBookByIdAsync(Guid id);
        Task<Result<Book>> UpdateBookByIdAsync(Guid id, UpdateBookRequest request);
    }
}
