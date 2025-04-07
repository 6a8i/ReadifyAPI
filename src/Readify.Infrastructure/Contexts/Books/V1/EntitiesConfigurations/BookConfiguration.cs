using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Readify.Application.Features.Books.V1.Infrastructure.Entities;

namespace Readify.Infrastructure.Contexts.Books.V1.EntitiesConfigurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            // Define o nome da tabela
            builder.ToTable("Books");

            // Define a chave primária
            builder.HasKey(b => b.Id);

            builder.Property(u => u.Id)
                .ValueGeneratedOnAdd();

            // Define propriedades
            builder.Property(b => b.Title)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(b => b.Author)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(b => b.Genre)
                   .HasMaxLength(100);

            builder.Property(b => b.PublishDate)
                   .HasColumnType("date");

            builder.Property(b => b.Status)
                   .HasDefaultValue(false);
        }
    }
}
