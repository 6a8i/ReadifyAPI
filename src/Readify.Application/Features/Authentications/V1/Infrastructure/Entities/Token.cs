using Readify.Application.Features.Users.V1.Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;

namespace Readify.Application.Features.Authentications.V1.Infrastructure.Entities
{
    public class Token
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool HasExpired { get; set; }

        public virtual User User { get; set; }
    }
}
