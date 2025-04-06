using Microsoft.EntityFrameworkCore;
using Readify.Application.Features.Books.V1.Infrastructure.Entities;

namespace Readify.Infrastructure.Commons.DatabaseContexts.V1
{
    public class ReadifyDatabaseContext : DbContext
    {
        public ReadifyDatabaseContext(DbContextOptions<ReadifyDatabaseContext> options) : base(options)
        {
            if (Database.IsRelational())
            {
                Database.EnsureCreated();
                Database.Migrate();
            }            
        }

        public virtual DbSet<Book> Books { get; set; } 
    }
}
