﻿using Readify.Application.Features.Books.V1.Infrastructure.Entities;

namespace Readify.Application.Features.Books.V1.Infrastructure.IRepositories
{
    public interface IBooksRepository
    {
        Task<Guid?> AddAsync(Book entity);
        Task<List<Book>> GetAllAsync();
        Task<Book?> GetBookByIdAsync(Guid id);
    }
}
