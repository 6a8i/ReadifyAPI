using Microsoft.EntityFrameworkCore;
using Readify.Application.Features.Books.V1.Infrastructure.Entities;

namespace Readify.Infrastructure.Commons.DatabaseContexts.V1
{
    public class ReadifyDatabaseContext : DbContext
    {
        public ReadifyDatabaseContext(DbContextOptions<ReadifyDatabaseContext> options) : base(options)
        {
            Database.EnsureCreated();
            Database.Migrate();
        }

        public DbSet<Book> Books { get; set; } 
    }
}
