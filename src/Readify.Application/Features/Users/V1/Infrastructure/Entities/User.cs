using Readify.Application.Features.Authentications.V1.Infrastructure.Entities;
using Readify.Application.Features.Users.V1.Models.Responses;
using System.ComponentModel.DataAnnotations;

namespace Readify.Application.Features.Users.V1.Infrastructure.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Token> Tokens { get; set; }

        public User()
        {
            Name = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            Tokens = new HashSet<Token>();
        }

        public static explicit operator User(GetUserResponse v)
        {
            return new User
            {
                Id = v.Id,
                Name = v.Name,
                Email = v.Email,
                CreatedAt = v.CreatedAt,
                IsActive = v.IsActive
            };
        }
    }
}
