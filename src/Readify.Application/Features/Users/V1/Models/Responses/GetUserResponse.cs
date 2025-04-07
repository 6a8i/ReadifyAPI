using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Readify.Application.Features.Users.V1.Models.Responses
{
    public class GetUserResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        public static explicit operator GetUserResponse(Infrastructure.Entities.User entity)
        {
            return new GetUserResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                CreatedAt = entity.CreatedAt,
                IsActive = entity.IsActive
            };
        }
    }
}
