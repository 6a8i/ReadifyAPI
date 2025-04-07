using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Readify.Application.Features.Users.V1.Infrastructure.Entities;

namespace Readify.Infrastructure.Contexts.Users.V1.EntitiesConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(b => b.Id);
            
            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd();

            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b => b.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.Password)
                .IsRequired();

            builder.Property(b => b.CreatedAt)
                .IsRequired();

            builder.Property(b => b.IsActive)
                   .HasDefaultValue(true);
        }
    }
}
