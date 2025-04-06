﻿using Readify.Application.Features.Books.V1.Infrastructure.Entities;
using Readify.Application.Features.Books.V1.Infrastructure.IRepositories;
using Readify.Infrastructure.Commons.DatabaseContexts.V1;

namespace Readify.Infrastructure.Contexts.Books.V1.Repositories
{
    class BooksRepository(ReadifyDatabaseContext context) : IBooksRepository
    {
        private readonly ReadifyDatabaseContext _context = context;

        public async Task<Guid?> AddAsync(Book entity)
        {
            await _context.Books.AddAsync(entity);
            
            var result = await _context.SaveChangesAsync();

            if (result > 0)
                return entity.Id;
            else
                return null;
        }
    }
}
