using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Readify.Application.Features.Authentications.V1.Infrastructure.Entities;

namespace Readify.Infrastructure.Contexts.Authentication.V1.EntitiesConfigurations
{
    public class TokenConfiguration : IEntityTypeConfiguration<Token>
    {
        public void Configure(EntityTypeBuilder<Token> builder)
        {
            builder.ToTable("Tokens");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd();

            builder.HasOne(b => b.User)
                .WithMany(b => b.Tokens)
                .HasForeignKey(b => b.UserId)
                .HasConstraintName("FK_Tokens_Users_UserId")   
                .OnDelete(DeleteBehavior.NoAction);

            builder.Property(b => b.CreatedAt)
                .IsRequired()
                .HasColumnType("Datetime2");

            builder.Property(b => b.ExpiresAt)
                .IsRequired()
                .HasColumnType("Datetime2");

            builder.Property(b => b.HasExpired)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(b => b.UserId)
                .IsRequired()
                .HasColumnType("uniqueidentifier");
        }
    }
}
