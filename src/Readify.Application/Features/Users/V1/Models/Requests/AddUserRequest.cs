using Readify.Application.Features.Books.V1.Models.Requests;
using Readify.Application.Features.Users.V1.Infrastructure.Entities;

namespace Readify.Application.Features.Users.V1.Models.Requests
{
    public class AddUserRequest
    {
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public static implicit operator User(AddUserRequest model)
        {
            return new User
            {
                Name = model.Name,
                BirthDate = model.BirthDate,
                Email = model.Email,
                Password = model.Password,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };
        }
    }
}
