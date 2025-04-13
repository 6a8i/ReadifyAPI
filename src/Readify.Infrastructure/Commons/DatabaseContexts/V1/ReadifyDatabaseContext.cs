using Microsoft.EntityFrameworkCore;
using Readify.Application.Features.Authentications.V1.Infrastructure.Entities;
using Readify.Application.Features.Books.V1.Infrastructure.Entities;
using Readify.Application.Features.Users.V1.Infrastructure.Entities;
using Readify.Infrastructure.Contexts.Authentication.V1.EntitiesConfigurations;
using Readify.Infrastructure.Contexts.Books.V1.EntitiesConfigurations;
using Readify.Infrastructure.Contexts.Users.V1.EntitiesConfigurations;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplica as configurações das entidades
            modelBuilder.ApplyConfiguration(new BookConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new TokenConfiguration());
        }

        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Token> Tokens { get; set; }
    }
}
