using Microsoft.EntityFrameworkCore;
using Readify.Application.Features.Books.V1.Infrastructure.Entities;
using Readify.Application.Features.Books.V1.Infrastructure.IRepositories;
using Readify.Infrastructure.Commons.DatabaseContexts.V1;

namespace Readify.Infrastructure.Contexts.Books.V1.Repositories
{
    public class BooksRepository(ReadifyDatabaseContext context) : IBooksRepository
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

        public async Task<List<Book>> GetAllAsync()
        {
            var result = await _context.Books.ToListAsync();

            return result;
        }

        public async Task<Book?> GetBookByIdAsync(Guid id)
        {
            var result = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);

            return result;
        }

        public async Task<Book?> UpdateAsync(Book entity)
        {
            var result = await _context
                                .Books
                                .Where(b => b.Id == entity.Id)
                                .ExecuteUpdateAsync(b =>
                                    b.SetProperty(p => p.Status, v => entity.Status)
                                     .SetProperty(p => p.Author, v => entity.Author)
                                     .SetProperty(p => p.Genre, v => entity.Genre)
                                     .SetProperty(p => p.Title, v => entity.Title)
                                     .SetProperty(p => p.PublishDate, v => entity.PublishDate));

            if (result > 0)
            {
                var updatedBook = await _context.Books.FirstOrDefaultAsync(b => b.Id == entity.Id);
                return updatedBook!;
            }
            else
            { 
                return null;
            }
        }
    }
}
